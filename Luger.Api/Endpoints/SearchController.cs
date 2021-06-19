using Luger.Api.Api.Models;
using Luger.Api.Endpoints.Models;
using Luger.Api.Features.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Luger.Api.Endpoints
{
    [Route("[controller]")]
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
            var logs = await logService.QueryLogs(bucket, searchRequest.From, searchRequest.To);

            return ApiResponse.FromData(new ResponseSearch { Logs = logs });
        }
    }
}
