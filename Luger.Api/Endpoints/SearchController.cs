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
        public async Task<ResponseSearch> GetAsync([FromRoute] string bucket, [FromBody] RequestSearch searchRequest)
        {
            await logService.QueryLogs(bucket, DateTimeOffset.Now, DateTimeOffset.Now);

            return null;
        }
    }
}
