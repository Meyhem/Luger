using Luger.Api.Common;
using Luger.Api.Endpoints.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Luger.Api.Features.Logging
{
    public class LogRecord
    {
        public static readonly string[] ReservedFields = new string[] { "_id", "level", "message", "timestamp" };

        public LogRecord() : this(null) { }

        public LogRecord(RequestAddLog? req)
        {
            Level = req?.Level ?? LogLevel.Error;
            Message = req?.Message ?? string.Empty;
            Labels = req?.Labels ?? new();
            Timestamp = DateTimeOffset.UtcNow;
        }

        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel Level { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
        public string Message { get; private set; }
        public Dictionary<string, string> Labels { get; private set; }

        public BsonDocument AsBsonDocument()
        {
            var d = new BsonDocument
            {
                { "level", new BsonString(Level.ToString()) },
                { "message", new BsonString(Message) },
                { "timestamp", new BsonDateTime(Timestamp.UtcDateTime) }
            };

            foreach (var label in Labels)
            {
                d.Add(label.Key, new BsonString(label.Value));
            }

            return d;
        }

        public static LogRecord? FromBsonDocument(BsonDocument d)
        {
            var log = new LogRecord();

            if (!Enum.TryParse<LogLevel>(d.GetStringOrEmpty("level"), out var level))
            {
                return null;
            }

            if (!d.TryGetValue("timestamp", out var timestampValue))
            {
                return null;
            }


            log.Level = level;
            log.Message = d.GetStringOrEmpty("message");
            log.Timestamp = new DateTimeOffset(timestampValue.ToUniversalTime());
            log.Labels = new();

            foreach (var key in d)
            {
                if (ReservedFields.Contains(key.Name)) continue;

                log.Labels.Add(key.Name, key.Value.AsString);
            }

            return log;
        }
    }
}
