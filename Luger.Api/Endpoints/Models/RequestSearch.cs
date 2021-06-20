using System;
using System.ComponentModel.DataAnnotations;

namespace Luger.Api.Endpoints.Models
{
    public class RequestSearch
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }

        public int Page { get; set; }
    }
}
