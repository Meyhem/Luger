using System;
using System.ComponentModel.DataAnnotations;

namespace Luger.Api.Api.Models
{
    public class RequestSearch
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public int Limit { get; set; }

        [Required]
        public Cursor Cursor { get; set; }
    }
}
