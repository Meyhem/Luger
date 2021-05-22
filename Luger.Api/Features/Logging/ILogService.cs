using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public interface ILogService
    {
        Task AddLogs(IEnumerable<LogRecord> logs);
    }
}