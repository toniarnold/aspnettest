using iie;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace minimal.Controllers
{
    public class MinimalController : Controller
    {
        private IConfigurationRoot Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public MinimalController(IConfigurationRoot config, IHostingEnvironment env, IHttpContextAccessor http)
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
                ViewBag.TestResult = testRunner.PassedString;
            }
            else
            {
                return this.Result();
            }

            return View("index");
        }

        /// <summary>
        /// View the test result as XML page
        /// </summary>
        /// <returns></returns>
        public IActionResult Result()
        {
            return Content(TestRunner.StaticResultString, "application/xml");
        }
    }
}