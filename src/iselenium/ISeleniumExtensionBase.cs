using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace iselenium
{
    /// <summary>
    /// Extension interface for a test fixture running in a web server process
    /// </summary>
    public interface ISeleniumBase : IAssertPoll
    {
#pragma warning disable IDE1006 // non-public members in Selenium-generated C# code
        IDictionary<string, object> vars { get; set; }
        IJavaScriptExecutor js { get; set; }
        IWebDriver driver { get; set; }
#pragma warning restore IDE1006
    }

    public static partial class SeleniumExtensionBase
    {
        internal const int FAST_POLL_MILLISECONDS = 20;  // > system quantum for context switches (10-15 ms)

        /// <summary>
        /// True when the TestRunner is started out of the web server process, e.g. by the NUnit3TestAdapter
        /// </summary>
        public static bool OutOfProcess { get; set; } = false;

        /// <summary>
        /// Port of the web development server to send callback HTTP requests to
        /// </summary>
        public static int Port { get; set; }

        /// <summary>
        /// MainControl.Response.StatusCode after the last request
        /// </summary>
        public static int StatusCode { get; set; }

        /// <summary>
        /// Timeout in seconds to wait for a HTTP response.
        /// </summary>
        public static int RequestTimeout { get; set; }

        /// <summary>
        /// Time interval in milliseconds between sending chars to a text input when > 0
        /// </summary>
        public static int WriteThrottle { get; set; }

        /// <summary>
        /// NYI in Selenium
        /// </summary>
        public static bool IEVisible { get; set; }

        /// <summary>
        /// [OneTimeSetUp]
        /// Start Internet Explorer and set up events
        /// </summary>
        public static void SetUpBrowser<TWebDriver>(this ISeleniumBase inst)
            where TWebDriver : IWebDriver, new()
        {
            if (typeof(IWebDriver) == typeof(InternetExplorerDriver))
            {
                var options = new InternetExplorerOptions();
                //options.IntroduceInstabilityByIgnoringProtectedModeSettings = true; // as the name says
                options.EnsureCleanSession = true;
                options.EnableNativeEvents = false;
                inst.driver = new InternetExplorerDriver(options);
            }
            else
            {
                inst.driver = new TWebDriver();
            }
            inst.js = (IJavaScriptExecutor)inst.driver;
            inst.vars = new Dictionary<string, object>();
            if (RequestTimeout > 0)
            {
                inst.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(RequestTimeout);
            }
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// Quit the browser
        /// </summary>
        public static void TearDownBrowser(this ISeleniumBase inst)
        {
            if (inst.driver != null)
            {
                inst.driver.Quit();
                inst.driver = null;
            }
        }

        /// <summary>
        /// Asynchronously issue a GET request for the specified absolute path at localhost
        /// <param name="delay">Optional delay time in seconds beforen navigating to the url</param>
        /// <param name="pause">Optional pause time in seconds after Selenium claims DocumentComplete</param>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        public static void Navigate(this ISeleniumBase inst, string path, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            Trace.Assert(path.StartsWith("/"), "path must be absolute");
            NavigateURL(inst, String.Format("http://localhost:{0}{1}", Port, path), expectedStatusCode, delay, pause);
        }

        public static void NavigateURL(this ISeleniumBase inst, string url, int expectedStatusCode = (int)HttpStatusCode.OK, int delay = 0, int pause = 0)
        {
            if (inst.driver == null)
            {
                throw new InvalidOperationException("IEExtension.SetUpIE() not called, you might want to inherit from IETest");
            }
            Thread.Sleep(delay);
            inst.driver.Navigate().GoToUrl(url);
            Thread.Sleep(pause);
            AssertStatusCode(inst, expectedStatusCode);
        }

        /// <summary>
        /// Get the HTML source of the page
        /// </summary>
        /// <param name="wait">Explicit WebDriverWait for the HTML to be rendered</param>
        /// <returns>HTML source</returns>
        public static string Html(this ISeleniumBase inst, int wait = 0)
        {
            var body = new WebDriverWait(inst.driver, TimeSpan.FromSeconds(wait))
                            .Until(drv => drv.FindElement(By.TagName("body")));
            var html = body.GetAttribute("outerHTML");
            return IIECompatible(inst) ? html.Replace("\r\n", "\n") : html;
        }

        /// <summary>
        /// Assert that there is no TestResult.xml with result="Failed" shown.
        /// Intended For running tests from ITestServer in a TestAdapter.
        /// </summary>
        public static void AssertTestsOK(this ISeleniumBase inst)
        {
            while (TestServerIPC.IsTestRunning)    // poll until the tests are finished
            {
                Thread.Sleep(1000); // 1 sec poll/await interval
            }
            switch (TestServerIPC.TestStatus)
            {
                case TestStatus.Failed:
                    Assert.Fail(TestServerIPC.TestResultXml);
                    break;

                case TestStatus.Inconclusive:
                    Assert.Inconclusive(TestServerIPC.TestResultXml);
                    break;

                case TestStatus.Passed:
                    Assert.Pass(TestServerIPC.TestResultXml);
                    break;

                case TestStatus.Skipped:
                    Assert.Ignore(TestServerIPC.TestResultXml);
                    break;

                case TestStatus.Warning:
                    Assert.Warn(TestServerIPC.TestResultXml);
                    break;

                default:     // TestStatus.Unknown
                    Assert.Warn(TestServerIPC.TestResultXml);
                    break;
            }
        }

        /// <summary>
        /// Click the HTML element (usually a Button) with the given name and
        /// index and wait for the response when expectPostBack is true.
        /// </summary>
        /// <param name="name">HTML name attribute of the element to click on</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page with the same HTML element</param>
        /// <param name="awaitRemoved">Whether to wait for the HTML element to disappear (in an SPA)</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        public static void ClickName(this ISeleniumBase inst, string name, int index = 0,
                                    bool expectRequest = true, bool samePage = false, bool awaitRemoved = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0)
        {
            var button = GetHTMLElementByName(inst, name, index, wait: wait);
            Click(button, awaitRemoved, delay, pause);
            if ((expectRequest || awaitRemoved) && samePage)
            {
                new WebDriverWait(inst.driver, TimeSpan.FromSeconds(RequestTimeout))
                    .Until(drv => drv.FindElement(By.Name(name)).Displayed);
            }
            AssertStatusCode(inst, expectedStatusCode);
        }

        /// <summary>
        /// Click the HTML element (usually a Button) with the given id
        /// and wait for the response when expectRequest is true.
        /// </summary>
        /// <param name="id">HTML id attribute of the element to click on</param>
        /// <param name="index">Index of the element collection with that id, defaults to 0</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page with the same HTML element</param>
        /// <param name="awaitRemoved">Whether to wait for the HTML element to disappear (in an SPA)</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>///
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        public static void ClickID(this ISeleniumBase inst, string id, int index = 0,
                                    bool expectRequest = true, bool samePage = false, bool awaitRemoved = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0,
                                    int wait = 0)
        {
            var button = GetHTMLElementById(inst, id, index, wait: wait);
            Click(button, awaitRemoved, delay, pause);
            if ((expectRequest || awaitRemoved) && samePage)
            {
                new WebDriverWait(inst.driver, TimeSpan.FromSeconds(RequestTimeout))
                    .Until(drv => drv.FindElement(By.Id(id)).Displayed);
            }
            AssertStatusCode(inst, expectedStatusCode);
        }

        /// <summary>
        /// Click on the HTML element
        /// </summary>
        /// <param name="element">The HTML element itself</param>
        /// <param name="awaitRemoved">Whether to wait for the HTML element to disappear (in an SPA)</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>///
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        private static void Click(IWebElement element, bool awaitRemoved, int delay, int pause)
        {
            Thread.Sleep(delay);
            element.Click();
            if (awaitRemoved)
            {
                AwaitElementRemoved(element);
            }
            Thread.Sleep(pause);
        }

        /// <summary>
        /// Write into the HTML element (usually a text input) with the given name
        /// </summary>
        /// <param name="name">HTML name attribute of the element to write to</param>
        /// <param name="text">Text to write</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        /// <param name="throttle">Time interval in milliseconds between sending chars to a text input when > 0</param>
        public static void Write(this ISeleniumBase inst, string name, string text, int index = 0,
                                int wait = 0, int throttle = 0)
        {
            var textinput = GetHTMLElementByName(inst, name, index, wait: wait);
            SendKeys(textinput, text, throttle);
        }

        /// <summary>
        /// Write into the HTML element (usually a text input) with the given id
        /// </summary>
        /// <param name="id">HTML id attribute of the element to click on</param>
        /// <param name="text">Text to write</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        /// <param name="throttle">Time interval in milliseconds between sending chars to a text input when > 0</param>
        public static void WriteID(this ISeleniumBase inst, string id, string text,
                                    int wait = 0, int throttle = 0)
        {
            var textinput = GetHTMLElementById(inst, id, wait: wait);
            SendKeys(textinput, text, throttle);
        }

        /// <summary>
        /// Select the item with the given value from the input element
        /// collection with the given id and wait for the response when
        /// expectPostBack is true.
        /// </summary>
        /// <param name="id">HTML id attribute of the element to click on</param>
        /// <param name="value">value of the item to click on</param>
        /// <param name="expectPostBack">Whether to expect a js-triggered server request from the click</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        public static void SelectID(this ISeleniumBase inst, string id, string value, bool expectPostBack = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0)
        {
            var list = GetHTMLElementsByID(inst, id, wait: wait);
            if (list == null)
            {
                throw new ArgumentException(String.Format("No HTML input elements with id='{0}' found", id));
            }
            for (int idx = 0; idx <= list.Count; idx++)
            {
                if (idx == list.Count)
                {
                    throw new ArgumentException(String.Format("HTML input element with id='{0}', value='{1}' not found", id, value));
                }
                else if (list[idx].GetAttribute("value") == value)
                {
                    ClickID(inst, id, idx, expectRequest: expectPostBack, samePage: !expectPostBack, expectedStatusCode: expectedStatusCode,
                            delay: delay, pause: pause);
                    if (expectPostBack)
                    {
                        new WebDriverWait(inst.driver, TimeSpan.FromSeconds(RequestTimeout))
                            .Until(drv => drv.FindElements(By.Id(id))[idx].Selected);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Select the item with the given value from the input element
        /// collection with the given name and wait for the response when
        /// expectPostBack is true.
        /// </summary>
        /// <param name="name">HTML name attribute of the element to click on</param>
        /// <param name="value">value of the item to click on</param>
        /// <param name="expectPostBack">Whether to expect a js-triggered server request from the click</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        public static void SelectName(this ISeleniumBase inst, string name, string value, bool expectPostBack = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0)
        {
            var list = GetHTMLElementsByName(inst, name, wait: wait);
            if (list == null)
            {
                throw new ArgumentException(String.Format("No HTML input elements with name='{0}' found", name));
            }
            for (int idx = 0; idx <= list.Count; idx++)
            {
                if (idx == list.Count)
                {
                    throw new ArgumentException(String.Format("HTML input element with name='{0}', value='{1}' not found", name, value));
                }
                else if (list[idx].GetAttribute("value") == value)
                {
                    ClickName(inst, name, idx, expectRequest: expectPostBack, samePage: !expectPostBack, expectedStatusCode: expectedStatusCode,
                            delay: delay, pause: pause);
                    if (expectPostBack)
                    {
                        new WebDriverWait(inst.driver, TimeSpan.FromSeconds(RequestTimeout))
                            .Until(drv => drv.FindElements(By.Name(name))[idx].Selected);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Get the element with the given id
        /// </summary>
        /// <param name="id">id resp. HTML id attribute of the element</param>
        /// <param name="index">Index of the element collection with that name, defaults to 0</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        /// <returns></returns>
        public static IWebElement GetHTMLElementById(this ISeleniumBase inst, string id, int index = 0, int wait = 0)
        {
            var elements = AwaitHTMLElements(inst, By.Id, id, wait);
            if (elements.Count <= index)
            {
                throw new ArgumentException(String.Format(
                   "HTML input element with id='{0}'{1} not found",
                        id, ((index) > 0) ? String.Format(" at index={0}", index) : ""));
            }
            return elements[index];
        }

        /// <summary>
        /// Get all input elements with the given id
        /// </summary>
        /// <param name="id">HTML id attribute of the elements</param>
        /// <param name="wait">Explicit WebDriverWait for the elements</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> GetHTMLElementsByID(this ISeleniumBase inst, string id, int wait = 0)
        {
            var elements = AwaitHTMLElements(inst, By.Id, id, wait);
            if (elements.Count == 0)
            {
                throw new ArgumentException(String.Format("HTML input element with id='{0}' not found", id));
            }
            else
            {
                return elements;
            }
        }

        /// <summary>
        /// Get the element with the given name at the given index
        /// When in IECompatible mode, name *or* id behavior mimics that of SHDocVw.InternetExplorer
        /// for backwards compatibility
        /// </summary>
        /// <param name="name">HTML name attribute of the element</param>
        /// <param name="index">Index of the element collection with that name, defaults to 0</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear</param>
        /// <returns></returns>
        public static IWebElement GetHTMLElementByName(this ISeleniumBase inst, string name, int index = 0, int wait = 0)
        {
            var elements = AwaitHTMLElements(inst, By.Name, name, wait);
            if (elements.Count <= index)
            {
                if (IIECompatible(inst))
                {
                    elements = AwaitHTMLElements(inst, By.Id, name, wait);
                    if (elements.Count <= index)
                    {
                        throw new ArgumentException(String.Format(
                            "HTML input element with name='{0}' or id='{0}'{1} not found",
                                name, ((index) > 0) ? String.Format(" at index={0}", index) : ""));
                    }
                    return elements[index];
                }
                throw new ArgumentException(String.Format(
                    "HTML input element with name='{0}'{1} not found",
                        name, ((index) > 0) ? String.Format(" at index={0}", index) : ""));
            }
            return elements[index];
        }

        /// <summary>
        /// Get all input elements with the given name
        /// </summary>
        /// <param name="name">name attribute of the elements</param>
        /// <param name="wait">Explicit WebDriverWait for the elements</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> GetHTMLElementsByName(this ISeleniumBase inst, string name, int wait = 0)
        {
            var elements = AwaitHTMLElements(inst, By.Name, name, wait);
            if (elements.Count == 0)
            {
                throw new ArgumentException(String.Format("HTML input element with name='{0}' not found", name));
            }
            else
            {
                return elements;
            }
        }

        /// <summary>
        /// On Chrome, WebDriverWait might sometimes find an element during DOM
        /// manipulation that has become stale. Ignore
        /// StaleElementReferenceException and try again.
        /// </summary>
        /// <param name="selector">delegate for By.Name(name) or By.Id(id)</param>
        /// <param name="nameOrId">argument for the selector</param>
        /// <param name="wait">Explicit WebDriverWait for the elements</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> AwaitHTMLElements(this ISeleniumBase inst,
                                                                        Func<string, By> selector,
                                                                        string nameOrId, int wait)
        {
            var elements = new ReadOnlyCollection<IWebElement>(Array.Empty<IWebElement>());
            for (int i = 0; i < RequestTimeout * 1000 / FAST_POLL_MILLISECONDS; i++)
            {
                try
                {
                    elements = new WebDriverWait(inst.driver, TimeSpan.FromSeconds(wait))
                                        .Until(drv => drv.FindElements(selector(nameOrId)));
                    if (elements.Count == 0)
                        return elements;            // element not findable within WebDriverWait -> give up
                    var _ = elements[0].Displayed;  // either the old one (throws) or the new one
                    return elements;                // didn't throw -> return
                }
                catch (StaleElementReferenceException) { } // next try for the non-stale element
                Thread.Sleep(FAST_POLL_MILLISECONDS);
            }
            return elements;
        }

        /// <summary>
        /// Explicitly wait for the element to disappear by rapidly polling
        /// its visibility until the reference has become stale or the
        /// RequestTimeout is exceeded.
        /// </summary>
        /// <param name="element">The HTML element that should disappear</param>
        private static void AwaitElementRemoved(IWebElement element)
        {
            bool isRemoved = false;
            for (int i = 0; i < RequestTimeout * 1000 / FAST_POLL_MILLISECONDS; i++)
            {
                try
                {
                    var _ = element.Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    isRemoved = true;
                    break;
                }
                Thread.Sleep(FAST_POLL_MILLISECONDS);
            }
            if (!isRemoved)
            {
                throw new TimeoutException(String.Format(
                            "AwaitElementRemoved took longer than {0}s, if this is expected set awaitRemoved: false",
                            RequestTimeout));
            }
        }

        internal static void SendKeys(IWebElement textinput, string text, int explicitthrottle)
        {
            int throttle = (explicitthrottle > 0) ? explicitthrottle : WriteThrottle;
            textinput.Clear();
            if (throttle == 0)
            {
                textinput.SendKeys(text);
            }
            else
            {
                for (int i = 0; i < text.Length; i++)
                {
                    textinput.SendKeys(text[i].ToString());
                    Thread.Sleep(throttle);
                }
            }
        }

        /// <summary>
        /// Asserts the status code while blindly accepting responses in the
        /// AnyOf list, currently 304 ("Not Modified")
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        private static void AssertStatusCode(this ISeleniumBase inst, int expectedStatusCode)
        {
            if (!SeleniumExtensionBase.OutOfProcess)
            {
                AssertPoll(inst, () => StatusCode, () => Is.AnyOf(expectedStatusCode,
                    (int)HttpStatusCode.NotModified));
            }
        }

        private static bool IIECompatible(this ISeleniumBase inst)
        {
            return inst is IIE;
        }
    }
}