using Luger.Api.Common;
using Luger.Api.Features.Logging.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public class LogRepository : ILogRepository
    {
        public const string DateTimeFormat = "yyyy-MM-dd-HH-mm-ss";

        public Stream OpenLogOutputStream(string bucket)
        {
            var bucketFolder = GetBucketFolder(bucket);
            Directory.CreateDirectory(bucketFolder);
            var now = DateTimeOffset.UtcNow;
            var newLogFileName = FormatChunkName(now);

            return File.Open(Path.Join(bucketFolder, newLogFileName), FileMode.Create, FileAccess.Write, FileShare.Read);
        }

        public Task<IEnumerable<string>> GetChunks(string bucket, DateTimeOffset from, DateTimeOffset to, string? startChunk) 
        { 
            var bucketFolder = GetBucketFolder(bucket);

            var parsedStartChunk = ParseChunkName(startChunk);

            var chunks = Directory.GetFiles(bucketFolder, "*.log")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Select(name => DateTimeOffset.ParseExact(name, DateTimeFormat, CultureInfo.InvariantCulture))
                .OrderBy(stamp => stamp)
                .Where(c => c >= from && c <= to)
                .SkipWhile(s => parsedStartChunk != null && s != parsedStartChunk)
                .Select(c => FormatChunkName(c))
                .AsEnumerable();

            return Task.FromResult(chunks);
        }

        public Stream OpenChunkStream(string bucket, string chunk)
        {
            var bucketFolder = GetBucketFolder(bucket);

            return File.Open(Path.Join(bucketFolder, chunk), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private string FormatChunkName(DateTimeOffset timestamp)
        {
            return $"{timestamp.ToString(DateTimeFormat)}.log";
        }

        private DateTimeOffset? ParseChunkName(string? chunk)
        {
            if (string.IsNullOrEmpty(chunk)) return null;

            chunk = Path.GetFileNameWithoutExtension(chunk);
            if (DateTimeOffset.TryParseExact(chunk, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStartChunk))
            {
                return parsedStartChunk;
            }

            return null;
        }

        private string GetBucketFolder(string bucket)
        {
            bucket = Normalization.NormalizeBucketName(bucket);
            return Path.Join("logs", bucket);
        }
    }
}
