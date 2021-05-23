using Google.Protobuf;
using Luger.Api.Common;
using Luger.Api.Features.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{

    public class LogWriterHostedService : BackgroundService
    {
        private readonly IConfigurationProvider config;
        private readonly ILogQueue queue;
        private readonly ILogRepository repository;
        private readonly ILogger<LogWriterHostedService> logger;
        private Dictionary<string, StoredLogOutputStream> bucketStreamMap;

        public LogWriterHostedService(IConfigurationProvider config, 
            ILogQueue queue, 
            ILogRepository repository, 
            ILogger<LogWriterHostedService> logger)
        {
            this.config = config;
            this.queue = queue;
            this.repository = repository;
            this.logger = logger;
            bucketStreamMap = new();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Starting queue processing");
                while (await queue.OutputAvailableAsync(cancellationToken))
                {
                    try
                    {
                        var log = await queue.ReceiveAsync(cancellationToken);
                        var logStream = GetLogStream(log.Bucket, log.Timestamp);

                        var storedLog = new StoredLog
                        {
                            Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(log.Timestamp),
                            Level = log.Level,
                            Message = log.Message
                        };
                        
                        storedLog.Labels.Add(Normalization.NormalizeLogLabels(log.Labels));

                        logStream.WriteLog(storedLog);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Log processing failed", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Queue processing failed", ex);
                throw;
            }
            finally
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Cancellation requested");
                }

                logger.LogInformation("Terminating queue processing");
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

            if (timedStream.CreatedAt.Add(config.GetBucketRotationFrequency(bucket)) < rotationBaseTime)
            {
                timedStream.Dispose();
                timedStream = new StoredLogOutputStream(repository.OpenLogStream(bucket));
                
                bucketStreamMap[bucket] = timedStream;
            }

            return timedStream;
        }
    }
}
