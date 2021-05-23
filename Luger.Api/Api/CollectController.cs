using Luger.Api.Api.Models;
using Luger.Api.Common;
using Luger.Api.Features.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luger.Api.Api
{
    [Route("[controller]")]
    public class CollectController : Controller
    {
        private readonly ILogService service;

        public CollectController(ILogService service)
        {
            this.service = service;
        }

        [HttpPost("{bucketName}")]
        public async Task<IActionResult> CollectAsync([FromBody]IEnumerable<AddLogRequest> logRequests, [FromRoute]string bucketName)
        {
            logRequests ??= Enumerable.Empty<AddLogRequest>();
            bucketName = Normalization.NormalizeBucketName(bucketName);

            var logs = logRequests.Select(lr => new LogRecord(bucketName, lr));

            await service.AddLogs(logs);

            return NoContent();
        }
    }
}
