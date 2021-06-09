using Luger.Api.Api.Models;
using Luger.Api.Endpoints.Models;
using Luger.Api.Features.Logging;
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

        [HttpPost("{bucketName}")]
        public async Task<ResponseSearch> GetAsync([FromRoute] string bucketName, [FromBody] RequestSearch searchRequest)
        {
            var str = DateTimeOffset.UtcNow.ToString("s");
            var cursorDto = searchRequest.Cursor.ToDto();
            var result = await logService.SearchLogs(bucketName,
                searchRequest.From,
                searchRequest.To,
                cursorDto);

            return new()
            {
                Logs = result,
                Cursor = Cursor.FromDto(cursorDto)
            };
        }
    }
}
