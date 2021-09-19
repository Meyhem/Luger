using System.Collections.Generic;
using System.Threading.Tasks;
namespace Luger.Features.Logging
{
    public interface ILogRepository
    {
        Task WriteLogs(string bucket, IEnumerable<Dictionary<string, object>> logs);
        Task FlushAsync();
    }
}