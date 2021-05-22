using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public interface ILogQueue
    {
        Task<bool> OutputAvailableAsync(CancellationToken ct);
        Task<LogRecord> ReceiveAsync(CancellationToken ct);
        Task SendAllAsync(IEnumerable<LogRecord> logs);
        Task SendAsync(LogRecord log);
    }
}