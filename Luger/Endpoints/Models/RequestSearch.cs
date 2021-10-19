using System;

namespace Luger.Endpoints.Models
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

        public string[] Levels { get; set; } = Array.Empty<string>();
        public string Message { get; set; } = string.Empty;

        public Label[] Labels { get; set; } = Array.Empty<Label>();

        public int Page { get; set; }
        public int PageSize { get; set; }

        public Cursor? Cursor { get; set; }
    }
}
