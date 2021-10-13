using System;
using System.Collections.Generic;
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
        private BucketSummary? cachedSummary;

        public BucketSummaryCalculator(ILogRepository logRepository)
        {
            this.logRepository = logRepository;
        }

        public async Task AddLogsAsync(LogRecordDto[] newLogs)
        {
            try
            {
                await semaphore.WaitAsync();
                
                var logSpan = newLogs[..MaxLogHistory];
                var overflowingCount = Math.Clamp(logList.Count + logSpan.Length - MaxLogHistory, 0, MaxLogHistory);
                for (var i = 0; i < overflowingCount; i++) logList.RemoveLast();
                foreach (var newLog in logSpan) logList.AddFirst(new LinkedListNode<LogRecordDto>(newLog));
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<BucketSummary> CalculateAsync(string bucket)
        {
            try
            {
                await semaphore.WaitAsync();
                
                if (lastStatCalculation.HasValue &&
                    lastStatCalculation.Value.Add(updateInterval) > DateTimeOffset.UtcNow &&
                    cachedSummary is not null) return cachedSummary;

                bucket = Normalization.NormalizeBucketName(bucket);
                
                cachedSummary = new BucketSummary();

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

        private void CalculateLogSummary(BucketSummary summary, LogRecordDto log)
        {
            switch (log.Level)
            {
                case LogLevel.Trace:
                    summary.TraceCount++;
                    break;
                case LogLevel.Debug:
                    summary.DebugCount++;
                    break;
                case LogLevel.Information:
                    summary.InformationCount++;
                    break;
                case LogLevel.Warning:
                    summary.WarningCount++;
                    break;
                case LogLevel.Error:
                    summary.ErrorCount++;
                    break;
                case LogLevel.Critical:
                    summary.CriticalCount++;
                    break;
                case LogLevel.None:
                    summary.NoneCount++;
                    break;
                default:
                    break;
            }
        }
        private void CalculateLogListSummary(BucketSummary summary, LinkedList<LogRecordDto> logs)
        {
            summary.CalculatedFromTimespan = logs.Last!.Value.Timestamp - logs.First!.Value.Timestamp;
        }
        private async Task CalculateBucketSizeAsync(BucketSummary summary, string bucket)
        {
            summary.BucketSize = await logRepository.EstimateBucketSizeAsync(bucket);
        }
    }
}