using asplib.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace iselenium
{
    /// <summary>
    /// Minimal base class for Selenium tests with a typed Controller accessor and a
    /// [OneTimeSetUp] / [OneTimeTearDown] pair for starting/stopping the
    /// browser
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TController">the Controller under test</typeparam>
    public abstract class SeleniumTest<TWebDriver, TController> : SeleniumTest<TWebDriver>, ISelenium
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// Typed accessor for the controller under test
        /// </summary>
        protected TController Controller
        {
            get
            {
                return (TController)StaticControllerExtension.GetController();
            }
        }

        /// <summary>
        /// Delete the global controller reference
        /// </summary>
        [TearDown]
        public void TearDownController()
        {
            StaticControllerExtension.TearDownController();
        }
    }
}