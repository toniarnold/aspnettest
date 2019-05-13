using iie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebSharper;

namespace minimal.websharper.web
{
    public static class Remoting
    {
        internal static IConfigurationRoot Configuration { get; set; }
        internal static IHostingEnvironment Environment { get; set; }

        private static DefaultHttpContext HttpContext
        {
            get
            {
                var ctx = WebSharper.Web.Remoting.GetContext();
                return (DefaultHttpContext)ctx.Environment["WebSharper.AspNetCore.HttpContext"];
            }
        }

        [Remote]
        public static async Task<string> Test()
        {
            var testRunner = new TestRunner(Configuration, Environment, (int)HttpContext.Request.Host.Port);
            await Task.Run(() => testRunner.Run("minimaltest.websharper.web"));
            return testRunner.SummaryHtml;
        }
    }
}
