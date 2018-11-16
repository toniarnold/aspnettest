using asp.Models;
using asplib.Controllers;
using iie;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace asp.Controllers
{
    public partial class Calculator : SmcController<CalculatorContext, CalculatorContext.CalculatorState>
    {
        private IHostingEnvironment Environment { get; }

        public Calculator(IConfigurationRoot config, IHostingEnvironment env, IHttpContextAccessor http) : base(config)
        {
            this.Construct();   // SMC part of the class
            Environment = env;
        }

        /// <summary>
        /// Main.ascx in WebForms
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Index(CalculatorViewModel model)
        {
            model.State = this.State;
            model.Stack = this.Stack;
            return View(model);
        }

        /// <summary>
        /// Run the test suite
        /// </summary>
        /// <returns></returns>
        public IActionResult Test(CalculatorViewModel model)
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

            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        /// <summary>
        /// View the test result as XML page
        /// </summary>
        /// <returns></returns>
        public IActionResult Result()
        {
            return Content(TestRunner.StaticResultString, "application/xml");
        }

        /// <summary>
        /// Footer.ascx.cs enterButton_Click
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Enter(CalculatorViewModel model)
        {
            // Corresponds to asp.calculator.View.Enter (but not possible in
            // the view itself in MVC Core): 
            // Locally throw a TestException for malicious input
            if (String.Compare(model.Operand, "except", true) == 0)
            {
                throw new TestException("Deliberate Exception");
            }

            this.Fsm.Enter(model.Operand);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);    // server side single page model
        }

        // Calculate.ascx.cs addButton_Click
        public ActionResult Add(CalculatorViewModel model)
        {
            this.Fsm.Add(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        // Calculate.ascx.cs subButton_Click
        public ActionResult Sub(CalculatorViewModel model)
        {
            this.Fsm.Sub(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        // Calculate.ascx.cs mulButton_Click
        public ActionResult Mul(CalculatorViewModel model)
        {
            this.Fsm.Mul(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        // Calculate.ascx.cs divButton_Click
        public ActionResult Div(CalculatorViewModel model)
        {
            this.Fsm.Div(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        // Calculate.ascx.cs powButton_Click
        public ActionResult Pow(CalculatorViewModel model)
        {
            this.Fsm.Pow(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        // Calculate.ascx.cs sqrtButton_Click
        public ActionResult Sqrt(CalculatorViewModel model)
        {
            this.Fsm.Sqrt(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        // Calculate.ascx.cs clrButton_Click
        public ActionResult Clr(CalculatorViewModel model)
        {
            this.Fsm.Clr(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }

        // Calculate.ascx.cs clrAllButton_Click
        public ActionResult ClrAll(CalculatorViewModel model)
        {
            this.Fsm.ClrAll(this.Stack);
            model.State = this.State;
            model.Stack = this.Stack;
            return View("Index", model);
        }
    }
}