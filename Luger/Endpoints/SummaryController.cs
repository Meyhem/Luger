using System.Threading.Tasks;
using Luger.Common;
using Luger.Endpoints.Models;
using Luger.Features.Summary;
using Microsoft.AspNetCore.Mvc;

namespace Luger.Endpoints
{
    [Route("api/[controller]")]
    public class SummaryController : LugerControllerBase
    {
        private readonly ISummaryService summaryService;

        public SummaryController(ISummaryService summaryService)
        {
            this.summaryService = summaryService;
        }

        [HttpGet("{bucket}")]
        public async Task<IActionResult> GetAsync(string bucket)
        {
            var result = await summaryService.GetBucketSummaryAsync(
                Normalization.NormalizeBucketName(bucket)
            );

            return Ok(BucketSummary.From(result));
        }
    }
}
