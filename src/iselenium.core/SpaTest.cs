using OpenQA.Selenium;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace iselenium
{
    /// <summary>
    /// Base class for Browser tests of SPA applications like WebSharper.
    /// Overrides some SeleniumExtensionBase methods with default expectPostBack: false
    /// and explicit wait for RequestTimeout seconds.
    /// No dependency on WebSharper specific, therefore in asplib.core.
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    public abstract class SpaTest<TWebDriver> : SeleniumTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// Globally set the awaitRemove: default for Click(), as simple SPA
        /// pages (as the minimal one) don't require awaitRemove: true, but
        /// complex ones on almost every clickable element which leads to
        /// another view without that element.
        /// </summary>
        protected bool awaitRemovedDefault = true;

        /// <summary>
        /// Globally set the samePage: default for Click(). True if the
        /// interaction with a single page with static elements is tested, false
        /// if the test jumps around several pages.
        /// Relevant if awaitRemovedDefault is true.
        /// </summary>
        protected bool samePageDefault = true;

        /// <summary>
        /// Start the browser and poll the root directory to be available (SPAs
        /// are known to have a long startup time)
        /// Without explicitly WebSharper.Web.Remoting.DisableCsrfProtection()
        /// this can prevent HTTP 403 errors on startup.
        /// </summary>
        public override void OneTimeSetUpBrowser()
        {
            this.SetUpBrowser<TWebDriver>();
            // When OutOfProcess is true, the tests are running in a TestAdapter in Visual Studio test runner,
            // and the browser gets started before the web server is running and polling can't start yet:
            if (!SeleniumExtensionBase.OutOfProcess)
            {
                this.PollHttpOK();
            }
        }

        private void PollHttpOK()
        {
            for (int i = 0;
                 i < SeleniumExtensionBase.RequestTimeout * 1000 / SeleniumExtensionBase.FAST_POLL_MILLISECONDS;
                 i++)
            {
                var rooturl = String.Format("http://localhost:{0}", SeleniumExtensionBase.Port);
                this.driver.Navigate().GoToUrl(rooturl);
                int[] ok = { (int)HttpStatusCode.OK, (int)HttpStatusCode.NotModified };
                if (ok.Contains(SeleniumExtensionBase.StatusCode))
                    break;
                Thread.Sleep(SeleniumExtensionBase.FAST_POLL_MILLISECONDS);
            }
        }

        /// <summary>
        /// Get the HTML source of the page, wait RequestTimeout seconds for the body element to appear
        /// </summary>
        /// <param name="wait">Explicit WebDriverWait for the HTML to be rendered</param>
        /// <returns>HTML source</returns>
        public string Html(int wait = 0)
        {
            return SeleniumExtensionBase.Html(this, wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
        }

        /// <summary>
        /// Click the HTML element (usually a Button) with the given id and
        /// index, don't wait for a response as expectRequest defaults to false for an SPA,
        /// but wait RequestTimeout seconds for the element to appear.
        /// </summary>
        /// <param id="id">HTML id attribute of the element to click on</param>
        /// <param name="index">Index of the element collection with that id, defaults to 0</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page with the same HTML element</param>
        /// <param name="awaitRemoved">Whether to wait for the HTML element to disappear (in an SPA)</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        /// <param name="wait">Explicit WebDriverWait in seconds  for the element to appear</param>
        public void Click(string id, int index = 0,
                            bool expectRequest = false, bool? samePage = null, bool? awaitRemoved = null,
                            int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0)
        {
            var doAwaitRemoved = awaitRemoved ?? this.awaitRemovedDefault;
            var onSamePage = samePage ?? this.samePageDefault;
            SeleniumExtensionBase.ClickID(this, id, index,
                                            expectRequest: expectRequest, samePage: onSamePage,
                                            awaitRemoved: doAwaitRemoved, expectedStatusCode: expectedStatusCode,
                                            delay: delay, pause: pause,
                                            wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
        }

        /// <summary>
        /// Write into the HTML element (usually a text input) with the given id, per default discrete.
        /// Wait RequestTimeout seconds for the element to appear.
        /// </summary>
        /// <param name="id">HTML id attribute of the element to click on</param>
        /// <param name="text">Text to write</param>
        /// <param name="wait">Explicit WebDriverWait in seconds  for the element to appear</param>
        /// <param name="throttle">Time interval in milliseconds between sending chars to a text input when > 0</param>
        public void Write(string id, string text,
                            int wait = 0, int throttle = 0)
        {
            SeleniumExtensionBase.WriteID(this, id, text,
                                            wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait,
                                            throttle);
        }

        /// <summary>
        /// Get the element with the given id
        /// Wait RequestTimeout seconds for the element to appear.
        /// </summary>
        /// <param name="id">HTML id attribute of the elements</param>
        /// <param name="index">Index of the element collection with that name, defaults to 0</param>
        /// <param name="wait">Explicit WebDriverWait in seconds  for the element to appear</param>
        /// <returns></returns>
        public IWebElement GetHTMLElementById(string id, int index = 0, int wait = 0)
        {
            return SeleniumExtensionBase.GetHTMLElement(this, By.Id, id, index,
                                            wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
        }
    }
}