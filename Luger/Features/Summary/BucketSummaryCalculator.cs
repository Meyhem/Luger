using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Logging;
using Luger.Features.Logging.Dto;
using Microsoft.Extensions.Logging;

namespace Luger.Features.Summary
{
    public class BucketSummaryCalculator
    {
        private const int MaxLogHistory = 1000;

        private readonly ILogRepository logRepository;
        private readonly TimeSpan updateInterval = TimeSpan.FromSeconds(5);
        private readonly LinkedList<LogRecordDto> logList = new();
        private readonly SemaphoreSlim semaphore = new(1, 1);

        private DateTimeOffset? lastStatCalculation;
        private BucketSummaryDto? cachedSummary;

        public BucketSummaryCalculator(ILogRepository logRepository)
        {
            this.logRepository = logRepository;
        }

        public async Task AddLogsAsync(LogRecordDto[] newLogs)
        {
            try
            {
                await semaphore.WaitAsync();
                
                var logSpan = newLogs.Take(MaxLogHistory).ToArray();
                var overflowingCount = Math.Clamp(logList.Count + logSpan.Length - MaxLogHistory, 0, MaxLogHistory);
                for (var i = 0; i < overflowingCount; i++) logList.RemoveFirst();
                foreach (var newLog in logSpan) logList.AddLast(new LinkedListNode<LogRecordDto>(newLog));
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<BucketSummaryDto> CalculateAsync(string bucket)
        {
            try
            {
                await semaphore.WaitAsync();
                
                if (lastStatCalculation.HasValue &&
                    lastStatCalculation.Value.Add(updateInterval) > DateTimeOffset.UtcNow &&
                    cachedSummary is not null) return cachedSummary;

                bucket = Normalization.NormalizeBucketName(bucket);
                
                cachedSummary = new BucketSummaryDto();
                cachedSummary.SampleSize = logList.Count;
                foreach (var log in logList) CalculateLogSummary(cachedSummary, log);

                // has at least two items
                if (logList.Count > 1) CalculateLogListSummary(cachedSummary, logList);
                
                await CalculateBucketSizeAsync(cachedSummary, bucket);
                
                lastStatCalculation = DateTimeOffset.UtcNow;

                return cachedSummary!;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void CalculateLogSummary(BucketSummaryDto summaryDto, LogRecordDto log)
        {
            switch (log.Level)
            {
                case LogLevel.Trace:
                    summaryDto.TraceCount++;
                    break;
                case LogLevel.Debug:
                    summaryDto.DebugCount++;
                    break;
                case LogLevel.Information:
                    summaryDto.InformationCount++;
                    break;
                case LogLevel.Warning:
                    summaryDto.WarningCount++;
                    break;
                case LogLevel.Error:
                    summaryDto.ErrorCount++;
                    break;
                case LogLevel.Critical:
                    summaryDto.CriticalCount++;
                    break;
                case LogLevel.None:
                    summaryDto.NoneCount++;
                    break;
                default:
                    break;
            }
        }
        private void CalculateLogListSummary(BucketSummaryDto summaryDto, LinkedList<LogRecordDto> logs)
        {
            summaryDto.CalculatedFromTimespan = logs.Last!.Value.Timestamp - logs.First!.Value.Timestamp;
        }
        private async Task CalculateBucketSizeAsync(BucketSummaryDto summaryDto, string bucket)
        {
            summaryDto.BucketSize = await logRepository.EstimateBucketSizeAsync(bucket);
        }
    }
}