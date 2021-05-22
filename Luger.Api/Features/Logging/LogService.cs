using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public class LogService : ILogService
    {
        private readonly ILogQueue queue;

        public LogService(ILogQueue queue)
        {
            this.queue = queue;
        }

        public async Task AddLogs(IEnumerable<LogRecord> logs)
        {
            await queue.SendAllAsync(logs);
        }
    }
}
