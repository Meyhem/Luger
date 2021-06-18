using Microsoft.Extensions.Configuration;
using System;

namespace Luger.Api.Features.Configuration
{
    public interface ILugerConfigurationProvider
    {
        byte[] GetIssuesSigningKey();
        IConfigurationSection GetJwtBearerOptions();
        UserOptions[] GetUsers();
    }
}