using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Luger.Common;
using Luger.Features.Configuration;
using Luger.Features.Logging;
using Luger.Features.Logging.FileSystem;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Luger
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptionsFor<LugerOptions>(services, "Luger");
            var jwtOptions = ConfigureOptionsFor<JwtOptions>(services, "Jwt");
            
            services.AddControllers();
            services.AddSwaggerGen();

            services.AddTransient<ILogService, LogService>();
            services.AddSingleton<ILogRepository, FileSystemLogRepository>();

            services.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwt =>
                {
                    jwt.IncludeErrorDetails = true;
                    jwt.TokenValidationParameters.ValidIssuer = jwtOptions.Issuer;
                    jwt.TokenValidationParameters.ValidAudience = jwtOptions.Audience;
                    jwt.TokenValidationParameters.IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));
                    jwt.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AssertConfigurationValid(app.ApplicationServices);
            
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.DocumentTitle = "Luger API";
            });
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.Map("api/{**path}", delegate (HttpContext ctx)
                {
                    ctx.Response.StatusCode = 404;
                    return Task.CompletedTask;
                });

                endpoints.MapFallbackToFile("index.html");
            });
        }
        
        private TOptions ConfigureOptionsFor<TOptions>(IServiceCollection collection, string key) where TOptions : class
        {
            var optionsSection = Configuration.GetSection(key);
            var options = optionsSection.Get<TOptions>();
            collection.AddOptions<TOptions>().Bind(optionsSection);
            return options;
        }

        private void AssertConfigurationValid(IServiceProvider di)
        {
            var lugerOptions = Configuration.GetSection("Luger").Get<LugerOptions>();
            var jwtOptions = Configuration.GetSection("Jwt").Get<JwtOptions>();
            var logger = di.GetRequiredService<ILogger<Startup>>();
            
            if (lugerOptions.Buckets.IsNullOrEmpty())
            {
                throw new ArgumentException(
                    "No buckets configured in json config. Configure at least one bucket in section \"Luger.Buckets\"");
            }

            foreach (var bucket in lugerOptions.Buckets)
            {
                if (!Regex.IsMatch(bucket.Id, "^[a-z0-9-_]+$"))
                {
                    throw new ArgumentException(
                        $"Bucket Id \"{bucket.Id}\" is invalid. Bucket Id may contain only lower case alphanumeric letters, dashes and underscores.");
                }
            }

            if (lugerOptions.Users.IsNullOrEmpty())
            {
                throw new ArgumentException(
                    "No users configured in json config. Configure at least one user in section \"Luger.Users\"");
            }

            foreach (var user in lugerOptions.Users)
            {
                if (!user.Buckets.All(userBucket => lugerOptions.Buckets.Any(bucket => bucket.Id == userBucket)))
                {
                    throw new ArgumentException(
                        $"User \"{user.Id}\" has access to non-existent bucket. Verify all to match to at least one Bucket Id");
                }
            }

            if (string.IsNullOrEmpty(lugerOptions.StorageDirectory))
            {
                throw new ArgumentException($"Luger.StorageDirectory is missing in configuration");
            }

            if (string.IsNullOrEmpty(jwtOptions.SigningKey))
            {
                throw new ArgumentException($"Jwt.SigningKey is missing in configuration");
            }

            foreach (var bucket in lugerOptions.Buckets)
            {
                logger.LogInformation("Bucket {Id} configured", bucket.Id);
            }

            foreach (var user in lugerOptions.Users)
            {
                logger.LogInformation("User {Id} configured", user.Id);
            }
        }
    }
}