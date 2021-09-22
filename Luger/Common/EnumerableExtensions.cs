using System.Collections.Generic;
using System.Linq;

namespace Luger.Common
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? self) => self is null || !self.Any();
    }
}