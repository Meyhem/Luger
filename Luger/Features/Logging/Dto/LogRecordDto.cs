using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Luger.Features.Logging.Dto
{
    public class LogRecordDto
    {
        public LogLevel Level { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string MessageTemplate { get; set; }
        public Dictionary<string, string> Labels { get; set; }
    }
}
