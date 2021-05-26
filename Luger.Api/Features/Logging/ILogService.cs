using Luger.Api.Features.Logging.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public interface ILogService
    {
        Task AddLogs(IEnumerable<LogRecord> logs);
        Task<IEnumerable<LogRecord>> SearchLogs(string bucket, DateTimeOffset from, DateTimeOffset to, CursorDto cursor);
    }
}