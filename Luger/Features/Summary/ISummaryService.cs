using System.Threading.Tasks;
using Luger.Features.Logging.Dto;

namespace Luger.Features.Summary
{
    public interface ISummaryService
    {
        Task AddLogsAsync(string bucket, LogRecordDto[] newLogs);
        Task<BucketSummary> GetBucketSummaryAsync(string bucket);
    }
}