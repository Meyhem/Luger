using Luger.Api.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace Luger.Api.Features.Configuration
{
    public class LugerConfigurationProvider : ILugerConfigurationProvider
    {
        private readonly IConfiguration config;

        public LugerConfigurationProvider(IConfiguration config)
        {
            this.config = config;
        }

        public BucketOptions? GetBucketConfiguration(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);

            var options = config.GetValue<LoggingOptions>("Luger");

            return options
                .Buckets?
                .Find(b => Normalization.NormalizeBucketName(b.Id) == bucket);
        }

        public byte[] GetIssuesSigningKey()
        {
            var key = config.GetSection("Jwt").GetValue<string>("TokenValidationParameters:IssuerSigningKey");
            return Encoding.UTF8.GetBytes(key);
        }

        public IConfigurationSection GetJwtBearerOptions()
        {
            return config.GetSection("Jwt");
        }
    }
}
