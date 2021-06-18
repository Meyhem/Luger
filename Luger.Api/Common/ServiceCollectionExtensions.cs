using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static T GetOptions<T>(this IServiceProvider sp) where T: class
        {
            return sp.GetRequiredService<IOptions<T>>().Value;
        }
    }
}
