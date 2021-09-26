using Luger.LoggerProvider;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Luger.Playground
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = new LugerLogProvider(new LugerLogOptions
            {
                LugerUrl = new Uri("http://localhost:7931"),
                Bucket = "project-1"
            });

            ILogger logger = provider.CreateLogger("Test category");

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
                logger.LogWarning("Test warning log {label} {category}", "labelvalue", "oops, dupe");
                await Task.Delay(500);
                
            }

            await Task.Delay(5000);
            
        }
    }
}
