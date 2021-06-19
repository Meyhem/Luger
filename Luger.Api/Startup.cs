using Luger.Api.Features.Configuration;
using Luger.Api.Features.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luger.Api
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
            services.AddControllers();
            services.AddSwaggerGen();

            services.AddTransient<ILugerConfigurationProvider, LugerConfigurationProvider>();
            services.AddTransient<ILogService, LogService>();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var jwtSection = Configuration.GetSection("Jwt");
                jwtSection.Bind(jwt);

                var key = jwtSection.GetValue<string>("TokenValidationParameters:IssuerSigningKey");
                jwt.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            });

            services.Configure<LoggingOptions>(Configuration.GetSection("Luger"));
            services.Configure<MongoOptions>(Configuration.GetSection("Luger:Mongo"));

            services.AddScoped(di =>
            {
                var opts = di.GetOptions<MongoOptions>();
                return new MongoClient(opts.Url);
            });

            services.AddScoped(di =>
            {
                var mc = di.GetRequiredService<MongoClient>();
                var opts = di.GetOptions<MongoOptions>();
                
                return mc.GetDatabase(opts.Database);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitDb(app.ApplicationServices).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.DocumentTitle = "Luger API";
            });

            app.UseRouting();
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private async Task InitDb(IServiceProvider di)
        {
            using var scope = di.CreateScope();
            di = scope.ServiceProvider;

            var service = di.GetRequiredService<ILogService>();
            await service.PrepareDatabase();
        }
    }
}