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
        private readonly IOptions<LoggingOptions> loggingOptions;
        private readonly IOptions<UserOptions[]> userOptions;

        public LugerConfigurationProvider(IConfiguration config, IOptions<LoggingOptions> loggingOptions)
        {
            this.config = config;
            this.loggingOptions = loggingOptions;
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

        public UserOptions[] GetUsers()
        {
            return loggingOptions.Value.Users ?? Array.Empty<UserOptions>();
        }
    }
}
