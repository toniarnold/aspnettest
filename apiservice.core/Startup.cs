using apiservice.Model.Db;
using apiservice.Services;
using asplib.Controllers;
using asplib.Model.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace apiservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ASP_DBEntities.ConnectionString = Configuration.GetConnectionString("ApiserviceDb");
        }

        public IConfiguration Configuration { get; }

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
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, StorageControllerActivator>());
            services.AddOptions();
            services.AddSingleton(Configuration);
            services.AddHttpContextAccessor();
            services.AddLogging();

            services.AddDbContext<AspserviceDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ApiserviceDb")));
            services.AddScoped<IAspserviceDb, AspserviceDb>();
            services.AddScoped<ISMSService, SMSServiceMock>();
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