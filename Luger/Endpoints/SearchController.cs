using System.Linq;
using System.Threading.Tasks;
using Luger.Endpoints.Models;
using Luger.Features.Logging;
using Luger.Features.Logging.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luger.Endpoints
{
    [Route("api/[controller]")]
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ILogService logService;

        public SearchController(ILogService logService)
        {
            this.logService = logService;
        }

        [HttpPost("{bucket}")]
        public async Task<ResponseSearch> GetAsync([FromRoute] string bucket, [FromBody] RequestSearch searchRequest)
        {
            var labelsDto = searchRequest.Labels
                .Where(l => !string.IsNullOrWhiteSpace(l.Name) && !string.IsNullOrWhiteSpace(l.Value))
                .Select(l => new LabelDto
                {
                    Name = l.Name!,
                    Value = l.Value!
                });

            var logs = await logService.QueryLogs(bucket,
                searchRequest.From,
                searchRequest.To,
                searchRequest.Levels,
                searchRequest.Message,
                labelsDto,
                searchRequest.Page,
                searchRequest.PageSize);

            return new ResponseSearch {Logs = logs};
        }
    }
}