using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Configuration;
using Luger.Features.Logging.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Luger.Features.Logging.FileSystem
{
    public class FileSystemLogRepository : ILogRepository, IDisposable, IAsyncDisposable
    {
        private readonly ILogger<FileSystemLogRepository> logger;
        private const string FileNameDateTimeFormat = "yyyy-MM-dd_HH-mm-ss";

        private readonly LugerOptions options;
        private readonly object writeLock;
        private readonly Task backgroundFlushingTask;

        private ConcurrentDictionary<string, ConcurrentQueue<LogRecordDto>> bucketQueueMap;
        private ConcurrentDictionary<string, FileStream?> bucketFileMap;
        private CancellationTokenSource cancellationTokenSource;

        public FileSystemLogRepository(IOptions<LugerOptions> options, ILogger<FileSystemLogRepository> logger)
        {
            this.logger = logger;
            this.options = options.Value;
            cancellationTokenSource = new CancellationTokenSource();
            bucketQueueMap = new ConcurrentDictionary<string, ConcurrentQueue<LogRecordDto>>();
            bucketFileMap = new ConcurrentDictionary<string, FileStream?>();

            foreach (var bucket in this.options.Buckets)
            {
                bucketQueueMap[Normalization.NormalizeBucketName(bucket.Id)] = new ConcurrentQueue<LogRecordDto>();
            }

            writeLock = new object();
            backgroundFlushingTask = FlushingBackgroundTask(cancellationTokenSource.Token);
        }

        public Task WriteLogsAsync(string bucket, IEnumerable<LogRecordDto> logs)
        {
            var queue = bucketQueueMap[Normalization.NormalizeBucketName(bucket)];

            foreach (var log in logs)
            {
                queue.Enqueue(log);
            }

            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<LogRecordDto> ReadLogs(string bucket, DateTimeOffset start, DateTimeOffset end)
        {
            bucket = Normalization.NormalizeBucketName(bucket);
            var bucketFolder = GetBucketFolder(bucket);

            if (!Directory.Exists(bucketFolder)) yield break;

            // select all files in bucket and parse their timestamps
            var orderedFileCandidates = Directory.GetFiles(bucketFolder)
                .Select(path => (Path: path, Stamp: ParseLogFileNameStamp(path)))
                .OrderBy(fileCandidate => fileCandidate.Stamp)
                .ToList();

            // find starting file
            var startIndexCandidate = orderedFileCandidates
                .FindIndex(fileCandidate => fileCandidate.Stamp > start);

            // if found take previous file
            // if not found take last file 
            startIndexCandidate = startIndexCandidate < 0 ? orderedFileCandidates.Count - 1 : Math.Max(startIndexCandidate - 1, 0);

            // filter candidates that can contain selected time range
            var filesToRead = orderedFileCandidates
                .Skip(startIndexCandidate)
                .Where(fileCandidate => fileCandidate.Stamp <= end)
                .Select(fileCandidate => fileCandidate.Path);

            foreach (var file in filesToRead)
            {
                logger.LogInformation("Reading file {File}", file);

                await using var fileStream = OpenLogFileRead(bucket, Path.GetFileName(file));
                var logStream = ReadLog(fileStream);
                using var logStreamEnumerator = logStream.GetEnumerator();

                for (;;)
                {
                    try
                    {
                        var hasNext = logStreamEnumerator.MoveNext();
                        if (!hasNext) break;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error reading file {File}", file);
                        break;
                    }

                    yield return logStreamEnumerator.Current;
                }
            }
        }

        public async Task FlushAsync()
        {
            var bucketLogsMap = new Dictionary<string, LogRecordDto[]>();

            lock (writeLock)
            {
                foreach (var (bucket, queue) in bucketQueueMap)
                {
                    bucketLogsMap[bucket] = queue.ToArray();
                    queue.Clear();
                }
            }

            var bucketWriteTasks = bucketLogsMap.Select(async bucketLogPair =>
            {
                var stream = await GetBucketStreamAsync(bucketLogPair.Key);
                foreach (var log in bucketLogsMap[bucketLogPair.Key])
                {
                    var jsonString = JsonConvert.SerializeObject(log, Formatting.None);

                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonString));
                    await stream.WriteAsync(Encoding.UTF8.GetBytes("\r\n"));
                }

                await stream.FlushAsync();
            });

            await Task.WhenAll(bucketWriteTasks);
        }

        private void DeleteExpiredFiles(BucketOptions bucketOptions)
        {
            var bucket = Normalization.NormalizeBucketName(bucketOptions.Id);
            var bucketFolder = GetBucketFolder(bucket);
            var now = DateTimeOffset.UtcNow;

            if (bucketOptions.MaxRetentionHours < 1) return;
            if (!Directory.Exists(bucketFolder)) return;

            var expiredFiles = Directory.GetFiles(bucketFolder)
                .Select(path => (Path: path, Stamp: ParseLogFileNameStamp(path)))
                .Where(pathStampPair =>
                    pathStampPair.Stamp.Add(TimeSpan.FromHours(bucketOptions.MaxRetentionHours)) < now)
                .Select(pathStampPair => pathStampPair.Path);

            foreach (var file in expiredFiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unable to delete file {Path}", file);
                }
            }
        }

        private IEnumerable<LogRecordDto> ReadLog(Stream stream)
        {
            var serializer = new JsonSerializer();

            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);
            jsonReader.SupportMultipleContent = true;

            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.StartObject)
                {
                    LogRecordDto? log = null;
                    try
                    {
                        log = serializer.Deserialize<LogRecordDto>(jsonReader);
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning("Corrupted log record on line {Line}. {Exception}",
                            jsonReader.LinePosition,
                            e);
                        continue;
                    }

                    if (log is not null) yield return log;
                }
            }
        }

        private async Task FlushingBackgroundTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var flushDelaySeconds = options.FlushIntervalSeconds == 0 ? 5 : options.FlushIntervalSeconds;
                    await Task.Delay(TimeSpan.FromSeconds(flushDelaySeconds), cancellationToken);

                    foreach (var bucketOptions in options.Buckets) DeleteExpiredFiles(bucketOptions);

                    await FlushAsync();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Flush task failed");
                }
            }
        }

        private async Task<Stream> GetBucketStreamAsync(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            bucketFileMap.TryGetValue(bucket, out var stream);

            if (stream is not null)
            {
                var logFileName = Path.GetFileName(stream.Name);
                var fileNameStamp = ParseLogFileNameStamp(logFileName);
                if (fileNameStamp.Add(TimeSpan.FromHours(1)) < DateTimeOffset.UtcNow)
                {
                    stream.Close();
                    await stream.DisposeAsync();
                    stream = null;
                }
            }

            if (stream is null)
            {
                stream = OpenLogFileWrite(bucket);
                bucketFileMap[bucket] = stream;
            }

            return stream;
        }

        private FileStream OpenLogFileWrite(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);
            var path = Path.Join(GetBucketFolder(bucket), CreateLogFileName());
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            return new FileStream(
                path: path,
                mode: FileMode.Create,
                access: FileAccess.Write,
                share: FileShare.Read | FileShare.Delete,
                bufferSize: 262144 // 256k
            );
        }

        private FileStream OpenLogFileRead(string bucket, string fileName)
        {
            bucket = Normalization.NormalizeBucketName(bucket);
            var path = Path.Join(GetBucketFolder(bucket), fileName);

            return new FileStream(
                path: path,
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.Read | FileShare.Write,
                bufferSize: 262144 // 256k
            );
        }

        private string GetBucketFolder(string bucket) =>
            Path.Join(options.StorageDirectory,
                Normalization.NormalizeBucketName(bucket));

        private static string CreateLogFileName() => $"{DateTimeOffset.UtcNow.ToString(FileNameDateTimeFormat)}.json";

        private static DateTimeOffset ParseLogFileNameStamp(string fileName)
        {
            if (DateTimeOffset.TryParseExact(Path.GetFileNameWithoutExtension(fileName),
                FileNameDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var timestamp))
            {
                return timestamp;
            }

            return DateTimeOffset.MinValue;
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            cancellationTokenSource.Cancel();
            await backgroundFlushingTask;

            foreach (var (bucket, file) in bucketFileMap)
            {
                if (file is null) continue;
                file.Close();
                await file.DisposeAsync();
                bucketFileMap[bucket] = null;
            }
        }
    }
}