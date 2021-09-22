using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luger.Features.Logging.Dto;
using Microsoft.Extensions.Logging;

namespace Luger.Features.Logging
{
    public interface ILogService
    {
        Task AddLogsAsync(string bucket, IEnumerable<LogRecordDto> logs);

        IAsyncEnumerable<LogRecordDto> QueryLogsAsync(string bucket,
            DateTimeOffset from,
            DateTimeOffset to,
            LogLevel[] levels,
            string message,
            LabelDto[] labels,
            int page,
            int pageSize);
    }
}