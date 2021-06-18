using Luger.Api.Endpoints.Models;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Luger.Api.Features.Logging
{
    public class LogRecord
    {
        public LogRecord(RequestAddLog req)
        {
            Level = req.Level ?? LogLevel.Error;
            Message = req.Message ?? string.Empty;
            Labels = req.Labels ?? new();
        }

        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public Dictionary<string, string> Labels { get; private set; }

        public BsonDocument AsBsonDocument()
        {
            var d = new BsonDocument
            {
                { "level", new BsonString(Level.ToString()) },
                { "message", new BsonString(Message) }
            };

            foreach (var label in Labels)
            {
                d.Add(label.Key, new BsonString(label.Value));
            }

            return d;
        }
    }
}
