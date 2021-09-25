using System;
using System.Collections.Generic;
using Luger.Features.Logging.Dto;

namespace Luger.Endpoints.Models
{
    public class ResponseSearch
    {
        public IEnumerable<LogRecordDto> Logs { get; set; } = Array.Empty<LogRecordDto>();
    }
}
