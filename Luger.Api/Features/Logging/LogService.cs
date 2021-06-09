using Luger.Api.Features.Logging.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public class LogService : ILogService
    {
        private readonly ILogQueue queue;
        private readonly ILogRepository repository;

        public LogService(ILogQueue queue, ILogRepository repository)
        {
            this.queue = queue;
            this.repository = repository;
        }

        public async Task AddLogs(IEnumerable<LogRecord> logs)
        {
            await queue.PutMany(logs);
        }

        public async Task<IEnumerable<LogRecord>> SearchLogs(string bucket, DateTimeOffset from, DateTimeOffset to, CursorDto cursor)
        {
            var chunks = await repository.GetChunks(bucket, from, to, cursor.Chunk);
            var result = new List<LogRecord>(cursor.Limit);

            if (!chunks.Any())
            {
                cursor.Chunk = null;
                cursor.Offset = 0;
            }
            
            foreach (var chunk in chunks)
            {
                if (result.Count == cursor.Limit) break;

                cursor.Chunk = chunk;
                var chunkStream = repository.OpenChunkStream(bucket, chunk);
                chunkStream.Seek(cursor.Offset, System.IO.SeekOrigin.Begin);

                while (result.Count != cursor.Limit)
                {
                    try
                    {
                        var parsedLog = StoredLog.Parser.ParseDelimitedFrom(chunkStream);
                        cursor.Offset = chunkStream.Position;
                        var logRecord = new LogRecord(bucket, parsedLog);
                        result.Add(logRecord);
                    }
                    catch(Google.Protobuf.InvalidProtocolBufferException)
                    {
                        cursor.Offset = 0;
                        cursor.Chunk = null;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
