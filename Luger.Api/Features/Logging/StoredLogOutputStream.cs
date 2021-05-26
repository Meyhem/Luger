using Google.Protobuf;
using System;
using System.IO;
using System.Timers;

namespace Luger.Api.Features.Logging
{
    public class StoredLogOutputStream : IDisposable
    {
        private readonly Stream stream;
        private readonly Timer flushTimer;

        public DateTimeOffset CreatedAt { get; private set; }

        public StoredLogOutputStream(Stream stream)
        {
            CreatedAt = DateTimeOffset.UtcNow;
            this.stream = stream;
            flushTimer = new Timer(TimeSpan.FromSeconds(3).TotalMilliseconds);
            flushTimer.Elapsed += delegate
            {
                stream.Flush();
            };
            flushTimer.Start();
        }

        public void WriteLog(StoredLog log)
        {
            log.WriteDelimitedTo(stream);
        }

        public void Dispose()
        {
            flushTimer.Stop();
            stream.Dispose();
        }
    }
}
