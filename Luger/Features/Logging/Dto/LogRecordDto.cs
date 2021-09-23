using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Luger.Features.Logging.Dto
{
    public class LogRecordDto
    {
        public const string TimestampField = "@t";
        public const string MessageField = "@m";
        public const string LevelField = "@l";

        [Newtonsoft.Json.JsonProperty(LevelField)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public LogLevel Level { get; set; }

        [Newtonsoft.Json.JsonProperty(TimestampField)] public DateTimeOffset Timestamp { get; set; }

        [Newtonsoft.Json.JsonProperty(MessageField)] public string Message { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonExtensionData] public Dictionary<string, object> Labels { get; set; } = new();

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