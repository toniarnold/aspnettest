using asplib.Controllers;
using asplib.Model;
using iie;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace asp.core
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public IHttpContextAccessor HttpContext { get; }

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
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
            services.AddMvc()
                    .AddControllersAsServices();
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, StorageControllerActivator>());
            services.AddOptions();
            services.AddSingleton(Configuration);
            services.AddSingleton(Environment);
            services.AddHttpContextAccessor();
            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();    // always for this demo
            app.UseExceptionHandler("/Error/Error");
            app.UseMiddleware<IIEMiddleware>(); // Global.asax
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Calculator}/{action=Index}/{id?}");
            });
        }
    }
}
