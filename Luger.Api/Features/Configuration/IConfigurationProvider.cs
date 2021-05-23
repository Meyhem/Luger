using System;

namespace Luger.Api.Features.Configuration
{
    public interface IConfigurationProvider
    {
        TimeSpan GetBucketRotationFrequency(string bucket);
    }
}