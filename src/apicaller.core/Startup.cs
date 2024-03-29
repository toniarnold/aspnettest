using apicaller.Services;
using asplib.Controllers;
using asplib.Model.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace apicaller
{
    public class Startup
    {
        public IConfiguration Configuration { get; } = default!;
        public IWebHostEnvironment Environment { get; } = default!;
        public IHttpContextAccessor HttpContext { get; } = default!;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ASP_DBEntities.ConnectionString = Configuration.GetConnectionString("ApiserviceDb");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, PersistentControllerActivator>());
            services.AddOptions();
            services.AddSingleton(Configuration);
            services.AddHttpContextAccessor();
            services.AddLogging();

            services.AddHttpClient();
            services.AddScoped<IServiceClient, ServiceClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();   // for PersistentController
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}