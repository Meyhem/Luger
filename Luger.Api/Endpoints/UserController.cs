using Luger.Api.Endpoints.Models;
using Luger.Api.Features.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Luger.Api.Endpoints
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILugerConfigurationProvider configurationProvider;

        public UserController(ILugerConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        [NonAction]
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> PostToken([FromBody] RequestCreateToken model)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var tokenFactory = new JwtSecurityTokenHandler();
            var key = configurationProvider.GetIssuesSigningKey();
            var jwtOpts = configurationProvider.GetJwtBearerOptions();

            var tok = tokenFactory.CreateToken(new()
            {
                Issuer = jwtOpts.GetValue<string>("Issuer"),
                Audience = jwtOpts.GetValue<string>("Audience"),
                Expires = DateTime.UtcNow.AddHours(1),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, model.UserId)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            });

            var str = tokenFactory.WriteToken(tok);

            return Ok(new
            {
                Token = str
            });
        }
    }
}
