﻿using asplib.Model;
using asplib.Remoting;
using iie;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using WebSharper.AspNetCore;

namespace asp.websharper.spa
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
                .AddSitelet(TestResult.Main);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage()
                .UseMiddleware<IIEMiddleware>()
                // Even this does not stop IE from sending 304 responses:
                .Use(async (httpContext, next) =>
                {
                    httpContext.Response.Headers[HeaderNames.CacheControl] = "no-cache";
                    httpContext.Response.Headers[HeaderNames.Pragma] = "no-cache";
                    httpContext.Response.Headers[HeaderNames.Expires] = "-1";
                    await next();
                })
                // From WebSharper:
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSharper()    //.UseWebSharper(builder => builder.UseSitelets(false))
                .UseSession()       // at the end to avoid "System.Security.Cryptography.CryptographicException: The payload was invalid."
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