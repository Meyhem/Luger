using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public interface ILogService
    {
        Task AddLogs(string bucket, IEnumerable<LogRecord> logs);
        Task PrepareDatabase();
        Task<IEnumerable<LogRecord>> QueryLogs(string bucket, DateTimeOffset from, DateTimeOffset to, string[] levels, int page, int pageSize);
    }
}