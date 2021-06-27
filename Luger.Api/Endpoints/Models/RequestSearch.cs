using System;

namespace Luger.Api.Endpoints.Models
{
    public class Label
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    public class RequestSearch
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }

        public string[] Levels { get; set; }
        public string Message { get; set; }

        public Label[] Labels { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
