using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Luger.Api.Features.Logging
{
    public class LogQueue : ILogQueue
    {
        private readonly BufferBlock<LogRecord> queue;

        public LogQueue()
        {
            queue = new(new()
            {
                EnsureOrdered = true
            });
        }

        public async Task<bool> OutputAvailableAsync(CancellationToken ct)
        {
            return await queue.OutputAvailableAsync(ct);
        }

        public async Task<LogRecord> ReceiveAsync(CancellationToken ct)
        {
            return await queue.ReceiveAsync(ct);
        }

        public async Task SendAsync(LogRecord log)
        {
            await queue.SendAsync(log);
        }

        public async Task SendAllAsync(IEnumerable<LogRecord> logs)
        {
            foreach (var log in logs)
            {
                await queue.SendAsync(log);
            }
        }
    }
}
