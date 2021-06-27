using Luger.Api.Api.Models;
using Luger.Api.Endpoints.Models;
using Luger.Api.Features.Logging;
using Luger.Api.Features.Logging.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Luger.Api.Endpoints
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ILogService logService;

        public SearchController(ILogService logService)
        {
            this.logService = logService;
        }

        [HttpPost("{bucket}")]
        public async Task<ApiResponse<ResponseSearch>> GetAsync([FromRoute] string bucket, [FromBody] RequestSearch searchRequest)
        {
            var labelsDto = searchRequest.Labels.Select(l => new LabelDto(l.Name, l.Value));

            var logs = await logService.QueryLogs(bucket, 
                searchRequest.From, 
                searchRequest.To, 
                searchRequest.Levels, 
                searchRequest.Message,
                labelsDto,
                searchRequest.Page, 
                searchRequest.PageSize);

            return ApiResponse.FromData(new ResponseSearch { Logs = logs });
        }
    }
}
