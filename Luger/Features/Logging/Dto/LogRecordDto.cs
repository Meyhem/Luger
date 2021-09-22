using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Luger.Features.Logging.Dto
{
    public class LogRecordDto
    {
        public const string TimestampField = "@t";
        public const string MessageField = "@m";
        public const string LevelField = "@l";
        public const string LabelsField = "@labels";

        [JsonProperty(LevelField)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel Level { get; set; }

        [JsonProperty(TimestampField)] public DateTimeOffset Timestamp { get; set; }

        [JsonProperty(MessageField)] public string Message { get; set; } = string.Empty;

        [JsonExtensionData] public Dictionary<string, object> Labels { get; set; } = new();

        public static LogRecordDto FromMap(Dictionary<string, string> map, DateTimeOffset time)
        {
            var dto = new LogRecordDto
            {
                Timestamp = time
            };

            map.TryGetValue(LevelField, out var l);
            dto.Level = Enum.TryParse<LogLevel>(l, out var level) ? level : LogLevel.Information;

            map.TryGetValue(MessageField, out var m);
            dto.Message = m ?? string.Empty;

            dto.Labels = new Dictionary<string, object>(
                map.Select(kv => KeyValuePair.Create(kv.Key, kv.Value as object))
            );
            dto.Labels.Remove(LevelField);
            dto.Labels.Remove(MessageField);

            return dto;
        }
    }
}