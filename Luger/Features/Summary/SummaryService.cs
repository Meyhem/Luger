using System.Threading.Tasks;
using Luger.Features.Logging.Dto;

namespace Luger.Features.Summary
{
    public class SummaryService : ISummaryService
    {
        private readonly ISummaryRepository summaryRepository;

        public SummaryService(ISummaryRepository summaryRepository)
        {
            this.summaryRepository = summaryRepository;
        }

        public Task AddLogsAsync(string bucket, LogRecordDto[] newLogs) => summaryRepository.AddLogs(bucket, newLogs);

        public Task<BucketSummaryDto> GetBucketSummaryAsync(string bucket) => summaryRepository.GetBucketSummary(bucket);
    }
}