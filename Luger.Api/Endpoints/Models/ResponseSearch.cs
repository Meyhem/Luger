using Luger.Api.Endpoints.Models;
using Luger.Api.Features.Logging;
using System.Collections.Generic;

namespace Luger.Api.Api.Models
{
    public class ResponseSearch
    {
        public IEnumerable<LogRecord> Logs { get; set; }
    }
}
