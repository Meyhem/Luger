using Luger.Api.Api.Models;
using Luger.Api.Common;
using Luger.Api.Features.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
        public IActionResult CollectAsync([FromBody]IEnumerable<AddLogRequest> logRequests, [FromRoute]string bucketName)
        {
            var x = ModelState;
            logRequests ??= Enumerable.Empty<AddLogRequest>();
            bucketName = Utils.NormalizeBucketName(bucketName);

            var logs = logRequests.Select(lr => new LogRecord(bucketName, lr));

            service.AddLogs(logs);

            return NoContent();
        }
    }
}
