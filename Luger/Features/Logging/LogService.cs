using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Logging.Dto;
using Microsoft.Extensions.Logging;

namespace Luger.Features.Logging
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> logger;
        private readonly ILogRepository logRepository;

        public LogService(ILogger<LogService> logger, ILogRepository logRepository)
        {
            this.logger = logger;
            this.logRepository = logRepository;
        }

        public async Task AddLogsAsync(string bucket, IEnumerable<LogRecordDto> logs)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            await logRepository.WriteLogsAsync(bucket, logs);
        }

        public async IAsyncEnumerable<LogRecordDto> QueryLogsAsync(string bucket,
            DateTimeOffset from,
            DateTimeOffset to,
            LogLevel[] levels,
            string message,
            LabelDto[] labels,
            int page,
            int pageSize)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            var toSkip = pageSize * page;
            var nlog = 0;

            await foreach (var log in logRepository.ReadLogs(bucket, from, to))
            {
                var isMatch = MatchesTimestampRange(log, from, to) &&
                              (levels.IsNullOrEmpty() || MatchesLevels(log, levels)) &&
                              (string.IsNullOrEmpty(message) || MatchesMessage(log, message)) &&
                              (labels.IsNullOrEmpty() || MatchesLabels(log, labels));

                if (!isMatch) continue;
                nlog++;
                if (nlog < toSkip) continue;

                yield return log;
            }
        }

        private bool MatchesTimestampRange(LogRecordDto log, DateTimeOffset from, DateTimeOffset to)
        {
            return log.Timestamp >= from && log.Timestamp <= to;
        }

        private bool MatchesLevels(LogRecordDto log, LogLevel[] levels)
        {
            return levels.Contains(log.Level);
        }

        private bool MatchesMessage(LogRecordDto log, string message)
        {
            var logMessage = log.Message ?? string.Empty;

            return logMessage.Contains(message, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool MatchesLabels(LogRecordDto log, LabelDto[] labels)
        {
            return labels.Any(queriedLabel =>
                log.Labels.Any(logLabel =>
                    !string.IsNullOrWhiteSpace(queriedLabel.Name) &&
                    !string.IsNullOrWhiteSpace(queriedLabel.Value) &&
                    logLabel.Key == queriedLabel.Name &&
                    logLabel.Value.ToString() == queriedLabel.Value
                    ||
                    string.IsNullOrWhiteSpace(queriedLabel.Name) &&
                    !string.IsNullOrWhiteSpace(queriedLabel.Value) &&
                    logLabel.Value.ToString() == queriedLabel.Value
                    ||
                    !string.IsNullOrWhiteSpace(queriedLabel.Name) &&
                    string.IsNullOrWhiteSpace(queriedLabel.Value) &&
                    logLabel.Key == queriedLabel.Name
                ));
        }
    }
}