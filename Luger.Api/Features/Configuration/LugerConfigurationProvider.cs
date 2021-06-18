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
            return config.GetValue<UserOptions[]>("Luger:Users");
        }
    }
}
