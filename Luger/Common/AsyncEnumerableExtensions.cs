using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luger.Common
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<IEnumerable<T>> ToEnumerableAsync<T>(this IAsyncEnumerable<T> self)
        {
            return await self.ToArrayAsync();
        }
        
        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> self)
        {
            return (await self.ToListAsync()).ToArray();
        }
        
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> self)
        {
            var list = new List<T>();
            await foreach (var item in self)
            {
                list.Add(item);
            }

            return list;
        }
    }
}