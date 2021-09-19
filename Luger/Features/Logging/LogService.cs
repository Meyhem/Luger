using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Configuration;
using Luger.Features.Logging.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Luger.Features.Logging
{
    public class LogService : ILogService
    {
        private const string TimestampField = "@t";
        private const string MessageTemplateField = "@mt";
        private const string LevelField = "@l";

        private readonly ILogger<LogService> logger;
        private readonly ILogRepository logRepository;
        private readonly IOptions<LugerOptions> options;

        public LogService(ILogger<LogService> logger, ILogRepository logRepository)
        {
            this.logger = logger;
            this.logRepository = logRepository;
        }

        public async Task AddLogs(string bucket, IEnumerable<Dictionary<string, string>> logs)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            var logObjects = logs.Select(log =>
            {
                var logObject = new Dictionary<string, object>(
                    log.Select(kv =>
                        KeyValuePair.Create(kv.Key, kv.Value as object)
                    )
                );

                logObject.TryGetValue(LevelField, out var levelString);
                if (!Enum.TryParse<LogLevel>(levelString as string ?? "", out var level))
                {
                    level = LogLevel.Information;
                }
                logObject[LevelField] = Enum.GetName(level)!;
                

                logObject.TryGetValue(MessageTemplateField, out var mt);
                logObject[MessageTemplateField] = mt ?? "";

                return logObject;
            });

            await logRepository.WriteLogs(bucket, logObjects);
        }

        public async Task<IEnumerable<LogRecordDto>> QueryLogs(string bucket,
            DateTimeOffset from,
            DateTimeOffset to,
            string[] levels,
            string message,
            IEnumerable<LabelDto> labels,
            int page,
            int pageSize)
        {
            // var col = db.GetCollection<BsonDocument>(bucket);
            // var filterBuilder = Builders<BsonDocument>.Filter;
            //
            // var levelsFilter = levels.Any() ? filterBuilder.AnyIn("level", levels) : filterBuilder.Empty;
            // var fromFilter = filterBuilder.Gte("timestamp", from.ToUnixTimeMilliseconds());
            // var toFilter = filterBuilder.Lte("timestamp", to.ToUnixTimeMilliseconds());
            // var messageFilter = !string.IsNullOrEmpty(message) ? filterBuilder.Regex("message", message) : filterBuilder.Empty;
            // var labelFilter = filterBuilder.And(
            //     labels.Select(l => filterBuilder.Regex(l.Name, l.Value ?? ""))
            // );
            //
            // var composedFilters = filterBuilder.And(levelsFilter, fromFilter, toFilter, messageFilter, labelFilter);
            //
            // var result = await col.FindAsync<BsonDocument>(composedFilters, new() { Limit = pageSize, Skip = page * pageSize });
            //
            // var ret = new List<LogRecordDto>(pageSize);
            // foreach (var r in result.ToEnumerable())
            // {
            //     try
            //     {
            //         var log = new LogRecordDto();
            //         if (log == null) continue;
            //
            //         ret.Add(log);
            //     }
            //     catch(Exception ex)
            //     {
            //         logger.LogError(ex, "Failed to process log", r);
            //     }
            // }

            return Enumerable.Empty<LogRecordDto>();
        }
    }
}