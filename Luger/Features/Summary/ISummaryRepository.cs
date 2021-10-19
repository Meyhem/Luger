using System.Threading.Tasks;
using Luger.Features.Logging.Dto;

namespace Luger.Features.Summary
{
    public interface ISummaryRepository
    {
        Task<BucketSummaryDto> GetBucketSummary(string bucket);
        Task AddLogs(string bucket, LogRecordDto[] newLogs);
    }
}