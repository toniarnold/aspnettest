using asplib.View;
using NUnit.Framework;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Minimal base class for Selenium tests with a [OneTimeSetUp] /
    /// [OneTimeTearDown] pair for starting/stopping the browser and a Main
    /// accessor
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TMain">the class Main under test</typeparam>
    public abstract class SeleniumTest<TWebDriver, TMain> : SeleniumTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
        where TMain : new()
    {
        /// <summary>
        /// The central access point made persistent across requests
        /// </summary>
        protected TMain Main
        {
            get { return this.MainControl.Main; }
        }

        protected IStorageControl<TMain> MainControl
        {
            get { return (IStorageControl<TMain>)ControlRootExtension.GetRoot(); }
        }

        /// <summary>
        /// Delete the global controller reference
        /// </summary>
        [TearDown]
        public void TearDownMainControl()
        {
            ControlRootExtension.TearDownRoot();
        }
    }
}