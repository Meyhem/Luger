using Luger.Api.Api.Models;
using Luger.Api.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Luger.Api.Features.Logging
{
    public class LogRecord
    {
        public LogRecord(string bucket, AddLogRequest req)
        {
            Timestamp = DateTimeOffset.UtcNow;
            Level = req.Level ?? LogLevel.Error;
            Message = req.Message ?? string.Empty;
            Labels = req.Labels ?? new();
            Bucket = Normalization.NormalizeBucketName(bucket);
        }

        public DateTimeOffset Timestamp { get; private set; }
        public string Bucket { get; set; }
        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public Dictionary<string, string> Labels { get; private set; }
    }
}
