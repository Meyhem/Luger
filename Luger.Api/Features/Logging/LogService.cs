using Luger.Api.Features.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<LoggingOptions> opts;
        private readonly ILogger<LogService> logger;

        public LogService(IMongoDatabase db, IOptions<LoggingOptions> opts, ILogger<LogService> logger)
        {
            this.db = db;
            this.opts = opts;
            this.logger = logger;
        }

        public async Task AddLogs(string bucket, IEnumerable<LogRecord> logs)
        {
            var col = db.GetCollection<BsonDocument>(bucket);

            await col.InsertManyAsync(logs.Select(l => l.AsBsonDocument()));
        }

        public async Task<IEnumerable<LogRecord>> QueryLogs(string bucket, DateTimeOffset from, DateTimeOffset to)
        {
            var col = db.GetCollection<BsonDocument>(bucket);
            var result = await col.FindAsync<BsonDocument>(Builders<BsonDocument>.Filter.Empty);

            var ret = new List<LogRecord>(500);

            foreach (var r in result.ToEnumerable())
            {
                try
                {
                    var log = LogRecord.FromBsonDocument(r);
                    if (log == null) continue;

                    ret.Add(log);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Failed to process log", r);
                }
            }

            return ret;
        }
         
        public async Task PrepareDatabase()
        {
            foreach (var b in opts.Value.Buckets)
            {
                var exists = (await db.ListCollectionsAsync(new ListCollectionsOptions
                {
                    Filter = new BsonDocument() { { "name", b.Id } }
                }))
                .Any();

                if (exists) continue;

                await db.CreateCollectionAsync(
                    b.Id, 
                    new() 
                    {
                        Capped =  b.MaxDocuments.HasValue || b.MaxSize.HasValue, 
                        MaxDocuments = b.MaxDocuments, 
                        MaxSize = b.MaxSize 
                    });
            }
        }
    }
}
