using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Configuration;
using Microsoft.Extensions.Options;

namespace Luger.Features.Logging.FileSystem
{
    using LogObject = Dictionary<string, object>;
    
    public class FileSystemLogRepository : ILogRepository, IDisposable, IAsyncDisposable
    {
        private const string FileNameDateTimeFormat = "yyyy-MM-dd_hh-mm-ss";

        private readonly LugerOptions options;
        private ConcurrentDictionary<string, ConcurrentQueue<LogObject>> bucketQueueMap;
        private ConcurrentDictionary<string, FileStream?> bucketFileMap;
        
        private readonly object writeLock;
        private CancellationTokenSource cancellationTokenSource;
        private readonly Task backgroundFlushingTask;

        public FileSystemLogRepository(IOptions<LugerOptions> options)
        {
            this.options = options.Value;
            cancellationTokenSource = new CancellationTokenSource();
            bucketQueueMap = new ConcurrentDictionary<string, ConcurrentQueue<LogObject>>();
            bucketFileMap = new ConcurrentDictionary<string, FileStream?>();
            
            foreach (var bucket in this.options.Buckets)
            {
                bucketQueueMap[Normalization.NormalizeBucketName(bucket.Id)] = new ConcurrentQueue<LogObject>();
            }
            
            writeLock = new object();
            backgroundFlushingTask = FlushingBackgroundTask(cancellationTokenSource.Token);
        }

        public Task WriteLogs(string bucket, IEnumerable<Dictionary<string, object>> logs)
        {
            var queue = bucketQueueMap[Normalization.NormalizeBucketName(bucket)];
            
            foreach (var log in logs)
            {
                queue.Enqueue(log);
            }
            
            return Task.CompletedTask;
        }

        public async Task FlushAsync()
        {
            var bucketLogsMap = new Dictionary<string, LogObject[]>();
            
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
                    var jsonString = JsonSerializer.Serialize(log, new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        IgnoreNullValues = true
                    });

                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonString));
                }

                await stream.FlushAsync();
            });

            await Task.WhenAll(bucketWriteTasks);
        }

        private async Task FlushingBackgroundTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var flushDelaySeconds = options.FlushIntervalSeconds == 0 ? 5 : options.FlushIntervalSeconds;
                await Task.Delay(TimeSpan.FromSeconds(flushDelaySeconds), cancellationToken);
                await FlushAsync();
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
                if (fileNameStamp.Add(TimeSpan.FromHours(1)) > DateTimeOffset.UtcNow)
                {
                    stream.Close();
                    await stream.DisposeAsync();
                    stream = null;
                }
            }
            
            if (stream is null)
            {
                stream = OpenLogFile(bucket);
                bucketFileMap[bucket] = stream;
            }

            return stream;
        }

        private FileStream OpenLogFile(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);
            var path = Path.Join(options.StorageDirectory, bucket, CreateLogFileName());
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            
            return new FileStream(path: path,
                mode: FileMode.Create,
                access: FileAccess.Write,
                share: FileShare.Read | FileShare.Delete,
                bufferSize: 262144); // 256k
        }

        private static string CreateLogFileName()
        {
            return $"{DateTimeOffset.UtcNow.ToString(FileNameDateTimeFormat)}.json";
        }

        private static DateTimeOffset ParseLogFileNameStamp(string fileName)
        {
            if (DateTimeOffset.TryParseExact(fileName,
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