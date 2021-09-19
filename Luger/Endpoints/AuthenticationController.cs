using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Luger.Endpoints.Models;
using Luger.Features.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Luger.Endpoints
{
    [Route("api/[controller]")]
    [Authorize]
    public class AuthenticationController : LugerControllerBase
    {
        private readonly LugerOptions lugerOptions;
        private readonly JwtOptions jwtOptions;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(IOptions<LugerOptions> lugerOptions,
            IOptions<JwtOptions> jwtOptions,
            ILogger<AuthenticationController> logger)
        {
            this.lugerOptions = lugerOptions.Value;
            this.jwtOptions = jwtOptions.Value;
            this.logger = logger;
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public IActionResult SignIn([FromBody] RequestCreateToken model)
        {
            logger.LogInformation("Signin attempt {UserId}", model.UserId);
            
            var user = lugerOptions.Users
                .FirstOrDefault(u => u.Id == model.UserId && u.Password == model.Password);

            if (user is null)
            {
                return Problem(title: "Invalid Credentials", statusCode: 400);
            }

            return Ok(new ResponseCreateToken
            {
                Token = CreateToken(user)
            });
        }

        [HttpPost]
        [Route("token/refresh")]
        public IActionResult RefreshToken()
        {
            var user = lugerOptions.Users
                .FirstOrDefault(u => u.Id == User.Identity?.Name);

            if (user is null)
            {
                return Problem(title: "Invalid Credentials", statusCode: 400);
            }

            return Ok(new ResponseCreateToken
            {
                Token = CreateToken(user)
            });
        }

        [NonAction]
        private string CreateToken(UserOptions user)
        {
            var tokenFactory = new JwtSecurityTokenHandler();

            user.Buckets ??= Array.Empty<string>();
            var key = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
            var tok = tokenFactory.CreateToken(new SecurityTokenDescriptor
            {

                Expires = DateTime.UtcNow.AddMinutes(1),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, user.Id),
                    new("Luger.Buckets", string.Join(',', user.Buckets))
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            });

            return tokenFactory.WriteToken(tok);
        }
    }
}