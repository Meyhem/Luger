using System.IO;

namespace Luger.Api.Features.Logging
{
    public interface ILogRepository
    {
        Stream OpenLogStream(string bucket);
    }
}