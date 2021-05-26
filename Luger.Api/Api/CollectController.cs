using Luger.Api.Api.Models;
using Luger.Api.Common;
using Luger.Api.Features.Configuration;
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
        private readonly ILugerConfigurationProvider configurationProvider;

        public CollectController(ILogService service, ILugerConfigurationProvider configurationProvider)
        {
            this.service = service;
            this.configurationProvider = configurationProvider;
        }

        [HttpPost("{bucketName}")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CollectAsync([FromBody]IEnumerable<RequestAddLog> logRequests, [FromRoute]string bucketName)
        {
            logRequests ??= Enumerable.Empty<RequestAddLog>();
            bucketName = Normalization.NormalizeBucketName(bucketName);

            var config = configurationProvider.GetBucketConfiguration(bucketName);
            if (config is null)
            {
                return NotFound("Bucket not found");
            }

            var logs = logRequests.Select(lr => new LogRecord(bucketName, lr));

            await service.AddLogs(logs);

            return NoContent();
        }
    }
}
