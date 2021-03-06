using System;
using System.Linq;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Endpoints.Models;
using Luger.Features.Logging;
using Luger.Features.Logging.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Luger.Endpoints
{
    [Route("api/[controller]")]
    // [Authorize]
    public class SearchController : LugerControllerBase
    {
        private readonly ILogService logService;

        public SearchController(ILogService logService)
        {
            this.logService = logService;
        }

        [HttpPost("{bucket}")]
        [ProducesResponseType(typeof(ResponseSearch), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> GetAsync([FromRoute] string bucket, [FromBody] RequestSearch searchRequest)
        {
            searchRequest ??= new RequestSearch();

            var labelsDto = searchRequest.Labels
                .Where(l => !string.IsNullOrWhiteSpace(l.Name) || !string.IsNullOrWhiteSpace(l.Value))
                .Select(l => new LabelDto
                {
                    Name = l.Name!,
                    Value = l.Value!
                });

            var cursor = new CursorDto
            {
                Shard = searchRequest.Cursor?.Shard ?? "",
                Offset = searchRequest.Cursor?.Offset ?? 0
            };

            var logs = logService.QueryLogsAsync(
                bucket,
                searchRequest.From,
                searchRequest.To,
                searchRequest.Levels.Select(Enum.Parse<LogLevel>).ToArray(),
                searchRequest.Message,
                labelsDto.ToArray(),
                searchRequest.Page,
                searchRequest.PageSize,
                cursor
            ).ToArrayAsync();

            return Ok(new ResponseSearch
            {
                Logs = await logs,
                Cursor = new Cursor
                {
                    Offset = cursor.Offset,
                    Shard = cursor.Shard
                }
            });
        }
    }
}