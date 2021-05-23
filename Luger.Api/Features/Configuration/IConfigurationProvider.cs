using System;

namespace Luger.Api.Features.Configuration
{
    public interface IConfigurationProvider
    {
        BucketOptions? GetBucketConfiguration(string bucket);
        TimeSpan GetBucketRotationFrequency(string bucket);
    }
}