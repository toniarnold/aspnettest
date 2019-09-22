using apicaller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace apitest.apicaller
{
    public static class ServiceProvider
    {
        private static IConfiguration _configuration;
        private static IServiceProvider _provider;

        public static IConfiguration Configuration
        {
            get
            {
                CreateProviderSingleton();
                return _configuration;
            }
        }

        public static T Get<T>()
        {
            CreateProviderSingleton();
            return (T)_provider.GetService(typeof(T));
        }

        private static readonly object lo = new object();

        private static void CreateProviderSingleton()
        {
            lock (lo)
            {
                if (_provider == null)
                {
                    _configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
                    var startup = new Startup(_configuration);
                    var sc = new ServiceCollection();
                    startup.ConfigureServices(sc);
                    _provider = sc.BuildServiceProvider();
                }
            }
        }
    }
}