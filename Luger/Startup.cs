using System;
using System.Text;
using System.Threading.Tasks;
using Luger.Features.Configuration;
using Luger.Features.Logging;
using Luger.Features.Logging.FileSystem;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
    }
}