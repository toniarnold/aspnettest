using asplib.Model.Db;
using asplib.Remoting;
using iselenium;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using WebSharper.AspNetCore;

namespace asp.websharper.spa
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public static HttpContext HttpContext { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            Environment = env;
            var dom = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            Configuration = dom;
            RemotingContext.Set(Environment, Configuration);
            ASP_DBEntities.ConnectionString = Configuration["ASP_DBEntities"];  // globally
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
                .AddSitelet(TestResultSite.Main);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                WebSharper.Web.Remoting.DisableCsrfProtection();    // Prevent HTTP 403 errors in GUI tests
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            app.UseMiddleware<ISeleniumMiddleware>()
                .UseSession()
                .UseMiddleware<RequestQuerySessionMiddleware>()
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSharper()
                .Run(context =>
                {
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