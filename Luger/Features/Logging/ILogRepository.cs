using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luger.Features.Logging.Dto;

namespace Luger.Features.Logging
{
    public interface ILogRepository
    {
        Task WriteLogsAsync(string bucket, IEnumerable<LogRecordDto> logs);
        Task FlushAsync();
        IAsyncEnumerable<LogRecordDto> ReadLogs(string bucket, DateTimeOffset start, DateTimeOffset end, CursorDto cursor);
        Task<long> EstimateBucketSizeAsync(string bucket);
    }
}