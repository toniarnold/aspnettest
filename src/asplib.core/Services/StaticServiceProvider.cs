using Microsoft.Extensions.DependencyInjection;
using System;

namespace asplib.Services
{
    public static class StaticServiceProvider
    {
        private static IServiceProvider _provider;

        public static void SetProvider(IServiceProvider provider)
        {
            _provider = provider;
        }

        public static T GetSingleton<T>()
        {
            return _provider.GetService<T>();
        }
    }
}