using System.Threading.Tasks;
using Luger.Features.Logging.Dto;

namespace Luger.Features.Summary
{
    public interface ISummaryService
    {
        Task AddLogs(string bucket, LogRecordDto[] newLogs);
        Task<BucketSummary> GetBucketSummary(string bucket);
    }
}