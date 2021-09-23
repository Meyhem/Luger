using System.Collections.Generic;

namespace Luger.Features.Configuration
{
    public class LugerOptions
    {
        public uint FlushIntervalSeconds { get; set; } = 5;
        public string StorageDirectory { get; set; } = "./logs";
        public List<BucketOptions> Buckets { get; set; } = default!;
        public UserOptions[] Users { get; set; }
    }

    public class JwtOptions
    {
        public string Issuer { get; set; } = "Luger";
        public string Audience { get; set; } = "Luger";
        public string SigningKey { get; set; }
        public int ExpiresSeconds { get; set; } = 3600;
    }
    
    public class BucketOptions
    {
        public string Id { get; set; }
    }

    public class UserOptions
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string[] Buckets { get; set; }
    }
}
