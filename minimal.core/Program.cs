using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace minimal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true");
    }
}