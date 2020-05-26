using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    /// <summary>
    /// Extension marker interface for a test fixture running in Internet Explorer
    /// </summary>
    [Obsolete("Replaced by ISelenium")]
    public interface IIE : ISeleniumBase
    {
    }

    /// <summary>
    /// Methods for managing the Internet Explorer instance
    /// </summary>
    [Obsolete("Replaced by SeleniumExtension with SetUpBrowser<IWebDriver>")]
    public static class IEExtension
    {
        /// <summary>
        /// To be used with ClickID()
        /// </summary>
        public const string EXCEPTION_LINK_ID = "exception-link";

        /// <summary>
        /// [OneTimeSetUp]
        /// Start Internet Explorer and set up events
        /// </summary>
        /// <param name="inst"></param>
        [Obsolete("Replaced by SetUpBrowser<IWebDriver>")]
        public static void SetUpIE(this ISeleniumBase inst)
        {
            SeleniumExtensionBase.SetUpBrowser<InternetExplorerDriver>(inst);
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// Quit Internet Explorer
        /// </summary>
        [Obsolete("Replaced by TearDownBrowser")]
        public static void TearDownIE(this ISeleniumBase inst)
        {
            SeleniumExtensionBase.TearDownBrowser(inst);
        }
    }
}