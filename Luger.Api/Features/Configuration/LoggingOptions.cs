using System;
using System.Collections.Generic;

namespace Luger.Api.Features.Configuration
{
    public class LoggingOptions
    {
        public List<BucketOptions> Buckets { get; set; } = default!;
    }

    public class BucketOptions
    {
        public string Name { get; set; }
        public TimeSpan? Rotation { get; set; }
        public TimeSpan? Retention { get; set; }
    }
}
