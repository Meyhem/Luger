using System;
using System.Net;

namespace Luger.LoggerProvider
{
    public class LugerLogOptions
    {
        public Uri LugerUrl { get; set; }
        public string Bucket { get; set; }
        public TimeSpan BatchPostInterval { get; set; } = TimeSpan.FromSeconds(3);

        public int MaxConnectionsPerServer { get; set; } = 3;
        public IWebProxy? Proxy { get; set; }
        public bool UseProxy { get; set; }
    }
}
