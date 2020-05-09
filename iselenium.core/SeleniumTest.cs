using asplib.Controllers;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Minimal base class for IE tests with a [OneTimeSetUp]/[OneTimeTearDown]
    /// pair for starting/stopping Internet Explorer and a static Controller
    /// accessor.
    /// </summary>
    public abstract class SeleniumTest<TWebDriver, TController> : SeleniumTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// Typed accessor for the controller under test
        /// </summary>
        protected TController Controller
        {
            get { return (TController)StaticControllerExtension.GetController(); }
        }
    }
}