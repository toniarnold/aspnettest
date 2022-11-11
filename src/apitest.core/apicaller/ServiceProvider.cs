using apicaller;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace apitest.apicaller
{
    /// <summary>
    /// apicaller
    /// </summary>
    public static class ServiceProvider
    {
        // Instantiated by lazy CreateMembers on each accessor
        private static IConfiguration _configuration = default!;

        private static IServiceProvider _provider = default!;

        public static IConfiguration Configuration
        {
            get
            {
                CreateMembers();
                return _configuration;
            }
        }

        public static T Get<T>()
        {
            CreateMembers();
            var inst = (T?)_provider.GetService(typeof(T));
            if (inst == null)
            {
                throw new Exception($"Service not registered: {typeof(T)}");
            }
            return (T)inst;
        }

        public static TestServer CreateTestServer()
        {
            CreateMembers();
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(_configuration)
                .UseStartup<Startup>();
            return new TestServer(builder);
        }

        private static readonly object lo = new object();

        private static void CreateMembers()
        {
            lock (lo)
            {
                if (_provider == null)
                {
                    _configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile(
                            Path.Combine("apicaller", "appsettings.json"),
                                optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables()
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