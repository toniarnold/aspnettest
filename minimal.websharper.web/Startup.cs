using asplib.Model.Db;
using iie;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebSharper.AspNetCore;

namespace minimal.websharper.web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public IHttpContextAccessor HttpContext { get; }

        public Startup(IWebHostEnvironment env)
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
            services.AddDistributedMemoryCache()
                .AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                })
                .AddLogging()
                // From WebSharper:
                .AddSitelet(Site.Main)
                .AddAuthentication("WebSharper")
                .AddCookie("WebSharper", options => { });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage()
                .UseSession()
                .UseMiddleware<IIEMiddleware>()
                // From WebSharper:
                .UseAuthentication()
                .UseStaticFiles()
                .UseWebSharper()
                .Run(context =>
                {
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