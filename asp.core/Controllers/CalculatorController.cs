using iie;
using asplib.Controllers;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace asp.Controllers
{
    public partial class Calculator : SerializableController
    {
        private IHostingEnvironment Environment { get; }

        public Calculator(IConfigurationRoot config, IHostingEnvironment env, IHttpContextAccessor http) : base(config)
        {
            this.Construct();   // SMC part of the class
            Environment = env;
        }

        public ActionResult Index()
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
            testRunner.Run("asptest.core");

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