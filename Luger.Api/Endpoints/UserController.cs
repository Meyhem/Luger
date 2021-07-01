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
using System.Threading;
using Microsoft.AspNetCore.Authorization;

namespace Luger.Api.Endpoints
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : LugerControllerBase
    {
        private readonly ILugerConfigurationProvider configurationProvider;

        public UserController(ILugerConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public IActionResult SignIn([FromBody] RequestCreateToken model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var user = configurationProvider.GetUsers()
                .FirstOrDefault(u => u.Id == model.UserId && u.Password == model.Password);

            if (user is null)
            {
                return BadRequest(
                    ApiResponse.FromError(
                        LugerError.From(LugerErrorCode.InvalidCredentials, "Invalid credentials")
                    )
                );
            }

            return Ok(ApiResponse.FromData(new ResponseCreateToken
            {
                Token = CreateToken(user)
            }));
        }

        [HttpPost]
        [Route("token/refresh")]
        public IActionResult RefreshToken()
        {

            var user = configurationProvider.GetUsers()
                .FirstOrDefault(u => u.Id == User.Identity?.Name);

            if (user is null)
            {
                return BadRequest(
                    ApiResponse.FromError(
                        LugerError.From(LugerErrorCode.InvalidCredentials, "Invalid credentials")
                    )
                );
            }

            return Ok(ApiResponse.FromData(new ResponseCreateToken
            {
                Token = CreateToken(user)
            }));
        }

        [NonAction]
        private string CreateToken(UserOptions user)
        {
            var tokenFactory = new JwtSecurityTokenHandler();
            var key = configurationProvider.GetIssuesSigningKey();
            var jwtOpts = configurationProvider.GetJwtBearerOptions();

            user.Buckets ??= Array.Empty<string>();

            var tok = tokenFactory.CreateToken(new()
            {
                Issuer = jwtOpts.GetValue<string>("Issuer"),
                Audience = jwtOpts.GetValue<string>("Audience"),

                Expires = DateTime.UtcNow.AddMinutes(1),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("Luger.Buckets", string.Join(',', user.Buckets))
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            });

            return tokenFactory.WriteToken(tok);
        }
    }
}
