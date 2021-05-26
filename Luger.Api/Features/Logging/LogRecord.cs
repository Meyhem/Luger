using Luger.Api.Api.Models;
using Luger.Api.Common;
using System;
using System.Collections.Generic;

namespace Luger.Api.Features.Logging
{
    public class LogRecord
    {
        public LogRecord(string bucket, RequestAddLog req)
        {
            Timestamp = DateTimeOffset.UtcNow;
            Level = req.Level ?? LogLevel.Error;
            Message = req.Message ?? string.Empty;
            Labels = req.Labels ?? new();
            Bucket = Normalization.NormalizeBucketName(bucket);
        }

        public LogRecord(string bucket, StoredLog log)
        {
            Timestamp = log.Timestamp.ToDateTimeOffset();
            Level = log.Level;
            Message = log.Message ?? string.Empty;
            Labels = new Dictionary<string, string>(log.Labels);
            Bucket = Normalization.NormalizeBucketName(bucket);
        }

        public DateTimeOffset Timestamp { get; private set; }
        public string Bucket { get; set; }
        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public Dictionary<string, string> Labels { get; private set; }
    }
}
