using Luger.Api.Features.Logging;
using System;
using System.Text.Json.Serialization;

namespace Luger.Api.Endpoints.Models
{
    public class RequestSearch
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }

        public string[] Levels { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
