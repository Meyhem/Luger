using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public class LogQueue : ILogQueue
    {
        private readonly Channel<LogRecord> queue;

        public LogQueue()
        {
            queue = Channel.CreateBounded<LogRecord>(65535);
        }

        public async Task<LogRecord> Get(CancellationToken ct)
        {
            return await queue.Reader.ReadAsync(ct);
        }

        public async Task Put(LogRecord log)
        {
            await queue.Writer.WriteAsync(log);
        }

        public async Task PutMany(IEnumerable<LogRecord> logs)
        {
            foreach (var log in logs)
            {
                await Put(log);
            }
        }
    }
}
