using System;
using System.Collections.Generic;

namespace Luger.Api.Features.Configuration
{
    public class LoggingOptions
    {
        public List<BucketOptions> Buckets { get; set; } = default!;
        public MongoOptions Mongo { get; set; }
    }

    public class BucketOptions
    {
        public string Id { get; set; }
        public int? MaxDocuments { get; set; }
        public int? MaxSize { get; set; }
    }

    public class UserOptions
    {
        public string Id { get; set; }
        public string[] Buckets { get; set; }
    }

    public class MongoOptions
    {
        public string Url { get; set; }
        public string Database { get; set; }
    }
}
