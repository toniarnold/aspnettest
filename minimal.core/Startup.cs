using asplib.Controllers;
using asplib.Model.Db;
using iselenium;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace minimal
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
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
                    .AddControllersAsServices();
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, PersistentControllerActivator>());
            services.AddOptions();
            services.AddSingleton(Configuration);
            services.AddSingleton(Environment);
            services.AddHttpContextAccessor();
            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();    // always for this demo
            app.UseExceptionHandler("/Error/Error");
            app.UseMiddleware<IIEMiddleware>(); // Global.asax
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Minimal}/{action=Index}/{id?}");
            });
        }
    }
}