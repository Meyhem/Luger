using Microsoft.Extensions.Configuration;
using System;

namespace Luger.Api.Features.Configuration
{
    public interface ILugerConfigurationProvider
    {
        BucketOptions? GetBucketConfiguration(string bucket);
        byte[] GetIssuesSigningKey();
        IConfigurationSection GetJwtBearerOptions();
    }
}