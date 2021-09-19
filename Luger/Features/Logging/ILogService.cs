using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luger.Features.Logging.Dto;

namespace Luger.Features.Logging
{
    public interface ILogService
    {
        Task AddLogs(string bucket, IEnumerable<Dictionary<string, string>> logs);
        Task<IEnumerable<LogRecordDto>> QueryLogs(string bucket, DateTimeOffset from, DateTimeOffset to, string[] levels, string message, IEnumerable<LabelDto> labels, int page, int pageSize);
    }
}