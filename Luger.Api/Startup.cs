using Luger.Api.Features.Configuration;
using Luger.Api.Features.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<ILogService, LogService>();
            services.AddSingleton<ILogQueue, LogQueue>();

            services.AddHostedService<LogWriterHostedService>();

            services.Configure<LoggingOptions>(Configuration.GetSection("Luger"));
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

            app.UseRouting();
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}