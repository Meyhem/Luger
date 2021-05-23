using Luger.Api.Common;
using Microsoft.Extensions.Options;
using System;

namespace Luger.Api.Features.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IOptions<LoggingOptions> options;

        public ConfigurationProvider(IOptions<LoggingOptions> options)
        {
            this.options = options;
        }

        public TimeSpan GetBucketRotationFrequency(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            var bucketConfig = options.Value?.Buckets?.Find(b => Normalization.NormalizeBucketName(b.Name) == bucket);
            return bucketConfig?.Rotation ?? TimeSpan.FromDays(1);
        }
    }
}
