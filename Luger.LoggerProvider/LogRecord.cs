using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Luger.LoggerProvider
{
    public class LogRecord
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel Level { get; set; }

        public string Message { get; set; } = default!;
        public Dictionary<string, string> Labels { get; set; } = new();
    }
}
