using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace asplib.Remoting
{
    /// <summary>
    /// IoC replacement to access services in a static Remoting class.
    /// In MVC core, these services are injected into the controller instances,
    /// whereas in WebSharper, there are no such instances (no .NET Core
    /// Dependency Injection is available without constructor).
    /// </summary>
    public static class RemotingContext
    {
        public static void Set(IWebHostEnvironment env, IConfiguration conf)
        {
            Environment = env;
            Configuration = conf;
        }

        public static IConfiguration Configuration { get; private set; }
        public static IWebHostEnvironment Environment { get; private set; }

        public static DefaultHttpContext HttpContext
        {
            get
            {
                var ctx = WebSharper.Web.Remoting.GetContext();
                return (DefaultHttpContext)ctx.Environment["WebSharper.AspNetCore.HttpContext"];
            }
        }

        public static int Port
        {
            get { return (int)RemotingContext.HttpContext.Request.Host.Port; }
        }
    }
}