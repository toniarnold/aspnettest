using iie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebSharper;

namespace minimal.websharper.spa
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

        /// <summary>
        /// JS Value class
        /// </summary>
        public class TestResult
        {
            public bool Passed;
            public string PassedString;
            public string ResultString;
        }

        [Remote]
        public static async Task<TestResult> Test()
        {

            var testRunner = new TestRunner(Configuration, Environment, (int)HttpContext.Request.Host.Port);
            await Task.Run(() => testRunner.Run("minimaltest.websharper.spa"));
            var result = new TestResult();
            result.Passed = testRunner.Passed;
            result.PassedString = testRunner.PassedString;
            result.ResultString = testRunner.ResultString;
            return result;
        }
    }
}
