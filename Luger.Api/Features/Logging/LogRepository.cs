using System;
using System.IO;

namespace Luger.Api.Features.Logging
{
    public class LogRepository : ILogRepository
    {
        public const string DateTimeFormat = "yyyy-MM-dd-HH-mm-ss";

        public Stream OpenLogStream(string bucket)
        {
            var bucketFolder = GetBucketFolder(bucket);
            Directory.CreateDirectory(bucketFolder);
            var now = DateTimeOffset.UtcNow;
            var newLogFileName = $"{now.ToString(DateTimeFormat)}.log";

            return File.Open(Path.Join(bucketFolder, newLogFileName), FileMode.Create, FileAccess.Write, FileShare.Read);
        }

        private string GetBucketFolder(string bucket)
        {
            return Path.Join("logs", bucket);
        }
    }
}
