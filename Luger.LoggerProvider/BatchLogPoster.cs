using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers;

namespace Luger.LoggerProvider
{
    public class BatchLogPoster: IDisposable
    {
        private readonly LugerLogOptions config;
        private readonly HttpClient httpClient;
        private readonly Queue<LogRecord> queue;
        private readonly object thislock;
        private readonly Timer timer;

        public BatchLogPoster(LugerLogOptions config, HttpClient httpClient)
        {
            timer = new Timer(config.BatchPostInterval.TotalMilliseconds);
            timer.Elapsed += delegate { TriggerBatchPost().Wait(); };
            timer.Start();

            thislock = new();
            queue = new Queue<LogRecord>(1024);
            this.config = config;
            this.httpClient = httpClient;
        }

        public void AddLog(LogRecord log)
        {
            lock (thislock)
            {
                queue.Enqueue(log);
            }
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();
        }

        private async Task TriggerBatchPost()
        {
            Console.WriteLine("Batchposting");
            LogRecord[] logs;
            lock (thislock)
            {
                logs = queue.ToArray();
                queue.Clear();
            }

            if (!logs.Any()) return;

            await httpClient.PostAsync($"/collect/{config.Bucket}", JsonContent.Create(logs));
        }
    }
}
