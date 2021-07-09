using Luger.Api.Endpoints.Models;
using Luger.Api.Features.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luger.Api.Endpoints
{
    [Route("api/[controller]")]
    public class CollectController : Controller
    {
        private readonly ILogService service;

        public CollectController(ILogService service)
        {
            this.service = service;
        }

        [HttpPost("{bucket}")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CollectAsync([FromRoute] string bucket, [FromBody] IEnumerable<RequestAddLog> logRequests)
        {
            logRequests ??= Enumerable.Empty<RequestAddLog>();

            var logs = logRequests.Select(lr => new LogRecord(lr));

            await service.AddLogs(bucket, logs);

            return NoContent();
        }
    }
}
