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
                Bucket = "bucket"
            });

            ILogger logger = provider.CreateLogger("Test category");
            var rng = new Random();
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("write");
                logger.Log((LogLevel)rng.Next(5), "This is test message! With label value={LabelValue}", i);
                
                await Task.Delay(100);
            }

            await Task.Delay(5000);
        }
    }
}
