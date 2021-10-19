using System.Collections.Concurrent;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Logging;
using Luger.Features.Logging.Dto;

namespace Luger.Features.Summary
{
    public class SummaryRepository : ISummaryRepository
    {
        private readonly ILogRepository logRepository;
        private readonly ConcurrentDictionary<string, BucketSummaryCalculator> stats = new();

        public SummaryRepository(ILogRepository logRepository)
        {
            this.logRepository = logRepository;
        }

        public async Task AddLogs(string bucket, LogRecordDto[] newLogs)
        {
            bucket = Normalization.NormalizeBucketName(bucket);
            
            var calculator = stats.GetOrAdd(bucket, _ => new BucketSummaryCalculator(logRepository));

            await calculator.AddLogsAsync(newLogs);
        }
        
        public async Task<BucketSummaryDto> GetBucketSummary(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            var calculator = stats.GetOrAdd(bucket, _ => new BucketSummaryCalculator(logRepository));

            return await calculator.CalculateAsync(bucket);
        }
    }
}