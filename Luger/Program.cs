using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("luger.json");
                    config.AddJsonFile("luger-override.json", optional: true);
                    config.AddJsonFile("luger-override-develop.json", optional: true);
                    if (bool.TryParse(Environment.GetEnvironmentVariable("RUNNING_IN_CONTAINER"),
                        out var isInContainer) && isInContainer)
                    {
                        config.AddJsonFile("/config/luger-override.json", optional: true);
                    }
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(c => c.SingleLine = true);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .Build()
                .Run();
        }
    }
}