using System;
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
            collection.Configure<TOptions>(optionsSection);
            var options = optionsSection.Get<TOptions>();

            return options;
        }

        private void AssertConfigurationValid(IServiceProvider di)
        {
            var lugerOptions = Configuration.GetSection("Luger").Get<LugerOptions>();

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
                    "No users configured in json config. Configure at least one bucket in section \"Luger.Users\"");
            }
        }
    }
}