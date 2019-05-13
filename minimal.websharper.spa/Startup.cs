using asplib.Controllers;
using asplib.Model;
using iie;
using iie.websharper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebSharper.AspNetCore;

namespace minimal.websharper.spa
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public static HttpContext HttpContext { get; set; }

        public Startup(IHostingEnvironment env)
        {

            Environment = env;
            var dom = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            Configuration = dom;
          

            ASP_DBEntities.ConnectionString = Configuration["ASP_DBEntities"];  // globally

            // Inject static references into the Remoting component
            Remoting.Configuration = Configuration;
            Remoting.Environment = Environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // WebSharper empty
            services.AddDistributedMemoryCache()
                .AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                })
                .AddLogging()
                .AddSitelet(TestResult.Main);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage()
                .UseSession()
                .UseMiddleware<IIEMiddleware>()
                // From WebSharper:
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSharper()    //.UseWebSharper(builder => builder.UseSitelets(false))
                .Run(context => {
                    HttpContext = context;
                    context.Response.StatusCode = 404;
                    return context.Response.WriteAsync("Page not found");
                });
        }

        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .Build()
                .Run();
        }
    }
}
