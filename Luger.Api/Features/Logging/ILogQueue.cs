using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public interface ILogQueue
    {
        Task<LogRecord> Get(CancellationToken ct);
        Task Put(LogRecord logs);
        Task PutMany(IEnumerable<LogRecord> logs);
    }
}