using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iie;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace minimal.core.Controllers
{
    public class MinimalController : Controller
    {
        private IConfigurationRoot Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public MinimalController(IConfigurationRoot config, IHostingEnvironment env)
        {
            Configuration = config;
            Environment = env;
        }

        public IActionResult Index()
        {
            return View();
        }
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
                return Content(testRunner.ResultString, "application/xml");
            }

            return View("index");
        }
    }
}