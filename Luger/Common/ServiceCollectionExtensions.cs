using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Luger.Common
{
    public static class ServiceCollectionExtensions
    {
        public static T GetOptions<T>(this IServiceProvider sp) where T: class
        {
            return sp.GetRequiredService<IOptions<T>>().Value;
        }
    }
}
