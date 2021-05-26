using Luger.Api.Features.Logging.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Luger.Api.Features.Logging
{
    public interface ILogRepository
    {
        Task<IEnumerable<string>> GetChunks(string bucket, DateTimeOffset from, DateTimeOffset to, string? startChunk);
        Stream OpenChunkStream(string bucket, string chunk);
        Stream OpenLogOutputStream(string bucket);
    }
}