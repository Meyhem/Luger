using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Luger.Api.Endpoints.Models;
using Luger.Api.Features.Configuration;
using System.Linq;

namespace Luger.Api.Endpoints
{
    [Route("api/[controller]")]
    public class UserController : LugerControllerBase
    {
        private readonly ILugerConfigurationProvider configurationProvider;

        public UserController(ILugerConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        [HttpPost]
        [Route("token")]
        public IActionResult CreateToken([FromBody] RequestCreateToken model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var tokenFactory = new JwtSecurityTokenHandler();
            var key = configurationProvider.GetIssuesSigningKey();
            var jwtOpts = configurationProvider.GetJwtBearerOptions();
            var users = configurationProvider.GetUsers();

            var user = users.FirstOrDefault(u => u.Id == model.UserId);

            if (user is null)
            {
                return BadRequest(
                    ApiResponse.FromError(
                        LugerError.From(LugerErrorCode.InvalidCredentials, "Invalid credentials")
                    )
                );
            }

            user.Buckets ??= Array.Empty<string>();

            var tok = tokenFactory.CreateToken(new()
            {
                Issuer = jwtOpts.GetValue<string>("Issuer"),
                Audience = jwtOpts.GetValue<string>("Audience"),
                Expires = DateTime.UtcNow.AddHours(1),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("Luger.Buckets", string.Join(',', user.Buckets))
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            });

            var str = tokenFactory.WriteToken(tok);

            return Ok(ApiResponse.FromData(new ResponseCreateToken
            {
                Token = str
            }));
        }
    }
}
