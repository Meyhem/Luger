using Google.Protobuf;
using Luger.Api.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{

    public class LogWriterHostedService : BackgroundService
    {
        private readonly IOptions<LoggingOptions> options;
        private readonly ILogQueue queue;
        private readonly ILogRepository repository;
        private readonly ILogger<LogWriterHostedService> logger;
        private Dictionary<string, StoredLogOutputStream> bucketStreamMap;

        public LogWriterHostedService(IOptions<LoggingOptions> options, ILogQueue queue, ILogRepository repository, ILogger<LogWriterHostedService> logger)
        {
            this.options = options;
            this.queue = queue;
            this.repository = repository;
            this.logger = logger;
            bucketStreamMap = new();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (await queue.OutputAvailableAsync(cancellationToken))
                {
                    try
                    {
                        var log = await queue.ReceiveAsync(cancellationToken);
                        var logStream = GetLogStream(log.Bucket, log.Timestamp);

                        var normalizedLabels = log.Labels
                            .Select(kvp => KeyValuePair.Create(
                                Utils.NormalizeLabelName(kvp.Key),
                                kvp.Value
                            ))
                            .GroupBy(g => g.Key)
                            .Select(g => g.First());

                        var storedLog = new StoredLog
                        {
                            Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(log.Timestamp),
                            Level = log.Level,
                            Message = log.Message
                        };

                        storedLog.Labels.Add(new Dictionary<string, string>(normalizedLabels));

                        logStream.WriteLog(storedLog);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Failed to write log", ex);
                    }
                }
            }
            finally
            {
                foreach (var (_, timedStream) in bucketStreamMap)
                {
                    timedStream.Dispose();
                }
                bucketStreamMap.Clear();
            }
        }


        private StoredLogOutputStream GetLogStream(string bucket, DateTimeOffset rotationBaseTime)
        {
            if (!bucketStreamMap.ContainsKey(bucket))
            {
                bucketStreamMap.Add(bucket, new StoredLogOutputStream(repository.OpenLogStream(bucket)));
            }
            var timedStream = bucketStreamMap[bucket];

            if (timedStream.CreatedAt.Add(GetBucketRotationFrequency(bucket)) < rotationBaseTime)
            {
                timedStream.Dispose();
                timedStream = new StoredLogOutputStream(repository.OpenLogStream(bucket));
                
                bucketStreamMap[bucket] = timedStream;
            }

            return timedStream;
        }

        private TimeSpan GetBucketRotationFrequency(string bucket)
        {

            var bucketConfig = options.Value?.Buckets?.Find(b => Utils.NormalizeBucketName(b.Name) == bucket);
            return bucketConfig?.Rotation ?? TimeSpan.FromDays(1);
        }


    }
}
