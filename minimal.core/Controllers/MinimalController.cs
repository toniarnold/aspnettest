using iselenium;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace minimal.Controllers
{
    public class MinimalController : Controller
    {
        private IConfigurationRoot Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public MinimalController(IConfigurationRoot config, IWebHostEnvironment env, IHttpContextAccessor http)
        {
            Configuration = config;
            Environment = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Run the test suite
        /// </summary>
        /// <returns></returns>
        public IActionResult Test()
        {
            var testRunner = new TestRunner(Configuration, Environment, (int)this.Request.Host.Port);
            testRunner.Run("minimaltest.core");

            if (testRunner.Passed)
            {
                ViewBag.TestResult = testRunner.SummaryHtml;
            }
            else
            {
                return this.ResultFailed();
            }

            return View("index");
        }

        /// <summary>
        /// View only the failed test results as XML page
        /// </summary>
        /// <returns></returns>
        public IActionResult ResultFailed()
        {
            return Content(TestRunner.ResultFailedXml, "application/xml; charset=UTF-8");
        }

        /// <summary>
        /// View the whole test result as XML page after clicking on the test summary
        /// </summary>
        /// <returns></returns>
        public IActionResult Result()
        {
            return Content(TestRunner.ResultXml, "application/xml; charset=UTF-8");
        }
    }
}