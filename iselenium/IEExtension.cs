using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    /// <summary>
    /// Extension marker interface for a test fixture running in Internet Explorer
    /// </summary>
    [Obsolete("Replaced by ISelenium")]
    public interface IIE : ISelenium
    {
    }

    /// <summary>
    /// Methods for managing the Internet Explorer instance
    /// </summary>
    [Obsolete("Replaced by SeleniumExtension with SetUpBrowser<TWebDriver>")]
    public static class IEExtension
    {
        /// <summary>
        /// [OneTimeSetUp]
        /// Start Internet Explorer and set up events
        /// </summary>
        /// <param name="inst"></param>
        public static void SetUpIE(this ISelenium inst)
        {
            SeleniumExtension.SetUpBrowser<InternetExplorerDriver>(inst);
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// Quit Internet Explorer
        /// </summary>
        public static void TearDownIE(this ISelenium inst)
        {
            SeleniumExtension.TearDownBrowser(inst);
        }
    }
}