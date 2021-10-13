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

        public Task AddLogs(string bucket, LogRecordDto[] newLogs) => summaryRepository.AddLogs(bucket, newLogs);

        public Task<BucketSummary> GetBucketSummary(string bucket) => summaryRepository.GetBucketSummary(bucket);
    }
}