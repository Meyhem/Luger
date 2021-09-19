using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Configuration;
using Luger.Features.Logging;
using Luger.Features.Logging.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Luger.Endpoints
{
    [Route("api/[controller]")]
    public class CollectController : Controller
    {
        private readonly ILogService service;
        private readonly LugerOptions options;

        public CollectController(ILogService service, IOptions<LugerOptions> options)
        {
            this.service = service;
            this.options = options.Value;
        }

        [HttpPost("{bucket}")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CollectAsync([FromRoute] string bucket, [FromBody] Dictionary<string, string>[]? logRequests)
        {
            logRequests ??= Array.Empty<Dictionary<string, string>>();
            if (options.Buckets.All(b => Normalization.NormalizeBucketName(b.Id) != Normalization.NormalizeBucketName(bucket)))
            {
                return Problem(detail: "No such bucket", statusCode: 404);
            }            

            await service.AddLogs(Normalization.NormalizeBucketName(bucket), logRequests);

            return NoContent();
        }
    }
}
