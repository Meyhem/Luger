using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public class LogService : ILogService
    {
        private readonly IMongoDatabase db;

        public LogService(IMongoDatabase db)
        {
            this.db = db;
        }

        public async Task AddLogs(string bucket, IEnumerable<LogRecord> logs)
        {
            var col = db.GetCollection<BsonDocument>(bucket);

            await col.InsertManyAsync(logs.Select(l => l.AsBsonDocument()));
        }

        public async Task<IEnumerable<LogRecord>> QueryLogs(string bucket, DateTimeOffset from, DateTimeOffset to)
        {
            return null;
        }
    }
}
