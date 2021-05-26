using Luger.Api.Common;
using Microsoft.Extensions.Options;
using System;

namespace Luger.Api.Features.Configuration
{
    public class LugerConfigurationProvider : ILugerConfigurationProvider
    {
        private readonly IOptions<LoggingOptions> options;

        public LugerConfigurationProvider(IOptions<LoggingOptions> options)
        {
            this.options = options;
        }

        public BucketOptions? GetBucketConfiguration(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            return options.Value?
                .Buckets?
                .Find(b => Normalization.NormalizeBucketName(b.Name) == bucket);
        }

        public TimeSpan GetBucketRotationFrequency(string bucket)
        {
            var bucketConfig = GetBucketConfiguration(bucket);

            return bucketConfig?.Rotation ?? TimeSpan.FromDays(1);
        }
    }
}
