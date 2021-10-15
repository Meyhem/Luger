using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Configuration;
using Luger.Features.Logging;
using Luger.Features.Logging.Dto;
using Luger.Features.Summary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Luger.Endpoints
{
    [Route("api/[controller]")]
    public class CollectController : LugerControllerBase
    {
        private readonly ILogService service;
        private readonly ISummaryService summaryService;
        private readonly LugerOptions options;

        public CollectController(ILogService service, ISummaryService summaryService, IOptions<LugerOptions> options)
        {
            this.service = service;
            this.summaryService = summaryService;
            this.options = options.Value;
        }

        [HttpPost("{bucket}")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CollectAsync([FromRoute] string bucket,
            [FromBody] Dictionary<string, string>[]? logs)
        {
            logs ??= Array.Empty<Dictionary<string, string>>();

            var mapped = logs.Select(l => LogRecordDto.FromMap(l, DateTimeOffset.UtcNow)).ToArray();

            await summaryService.AddLogsAsync(bucket, mapped);
            
            await service.AddLogsAsync(
                Normalization.NormalizeBucketName(bucket),
                mapped
            );

            return NoContent();
        }
    }
}