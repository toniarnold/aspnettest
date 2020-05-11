using asplib.Controllers;
using NUnit.Framework;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Minimal base class for Selenium tests with a [OneTimeSetUp]/[OneTimeTearDown]
    /// pair for starting/stopping the browser and a Controller accessor.
    /// </summary>
    public abstract class SeleniumTest<TWebDriver, TController> : SeleniumTestBase<TWebDriver>, ISelenium
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// Typed accessor for the controller under test
        /// </summary>
        protected TController Controller
        {
            get { return (TController)StaticControllerExtension.GetController(); }
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