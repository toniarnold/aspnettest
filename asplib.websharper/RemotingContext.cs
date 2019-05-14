using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebSharper;

namespace asplib
{
    /// <summary>
    /// IoC replacement to access services in a static Remoting class 
    /// In MVC core, these services are injected into the controller instances,
    /// whereas in WebSharper, there are no such instances (no .NET Core
    /// DependencyInjection available without constructor).
    /// </summary>
    public static class RemotingContext
    {
        public static void Set(IHostingEnvironment env, IConfigurationRoot conf)
        {
            Environment = env;
            Configuration = conf;
        }

        public static IConfigurationRoot Configuration { get; private set;  }
        public static IHostingEnvironment Environment { get; private set; }
        public static int Port
        {
            get { return (int)RemotingContext.HttpContext.Request.Host.Port; }
        }
        public static DefaultHttpContext HttpContext
        {
            get
            {
                var ctx = WebSharper.Web.Remoting.GetContext();
                return (DefaultHttpContext)ctx.Environment["WebSharper.AspNetCore.HttpContext"];
            }
        }
    }
}
