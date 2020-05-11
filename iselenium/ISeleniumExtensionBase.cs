using NUnit.Framework;
using OpenQA.Selenium;
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
    public interface ISeleniumBase
    {
#pragma warning disable IDE1006 // non-public members in Selenium-generated C# code
        IDictionary<string, object> vars { get; set; }
        IJavaScriptExecutor js { get; set; }
        IWebDriver driver { get; set; }
#pragma warning restore IDE1006
    }

    public static class SeleniumExtensionBase
    {
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
        /// NYI in Selenium
        /// </summary>
        public static bool IEVisible { get; set; }

        /// <summary>
        /// [OneTimeSetUp]
        /// Start Internet Explorer and set up events
        /// </summary>
        /// <param name="inst"></param>
        public static void SetUpBrowser<TWebDriver>(this ISeleniumBase inst)
            where TWebDriver : IWebDriver, new()
        {
            inst.driver = new TWebDriver();
            inst.js = (IJavaScriptExecutor)inst.driver;
            inst.vars = new Dictionary<string, object>();

            inst.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(RequestTimeout);
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// Quit Internet Explorer
        /// </summary>
        public static void TearDownBrowser(this ISeleniumBase inst)
        {
            inst.driver.Quit();
            inst.driver = null;
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
            AssertStatusCode(expectedStatusCode);
        }

        /// <summary>
        /// Get the HTML source of the page
        /// </summary>
        /// <returns></returns>
        public static string Html(this ISeleniumBase inst)
        {
            return inst.driver.FindElement(By.TagName("body")).GetAttribute("outerHTML");
        }

        /// <summary>
        /// Click the HTML element (usually a Button) with the given name and
        /// index and wait for the response when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="name">HTML name attribute of the element to click on</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void ClickName(this ISeleniumBase inst, string name, int index = 0,
                                    bool expectRequest = true, bool samePage = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElementByName(inst, name, index);
            Click(inst, button, expectRequest: expectRequest, delay: delay, pause: pause);
            if (expectRequest && samePage)
            {
                var _ = new WebDriverWait(inst.driver, TimeSpan.FromSeconds(RequestTimeout))
                            .Until(drv => drv.FindElement(By.Name(name)).Displayed);
            }
            AssertStatusCode(expectedStatusCode);
        }

        /// <summary>
        /// Click the HTML element (usually a Button) with the given clientID
        /// and wait for the response when expectPostBack is true.
        /// </summary>
        /// <param name="clientId">HTML id attribute of the element to click on</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>///
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void ClickID(this ISeleniumBase inst, string clientId,
                                    bool expectRequest = true, bool samePage = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElementById(inst, clientId);
            Click(inst, button, expectRequest: expectRequest, delay: delay, pause: pause);
            if (samePage)
            {
                var _ = new WebDriverWait(inst.driver, TimeSpan.FromSeconds(RequestTimeout))
                            .Until(drv => drv.FindElement(By.Id(clientId)).Displayed);
            }
            AssertStatusCode(expectedStatusCode);
        }

        /// <summary>
        /// Click on the HTML element and wait for the response when expectRequest is true.
        /// </summary>
        /// <param name="element">The HTML element itself</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        private static void Click(this ISeleniumBase inst, IWebElement element, bool expectRequest = true,
                                    int delay = 0, int pause = 0)
        {
            Thread.Sleep(delay);
            element.Click();
            if (expectRequest)
            {
                AwaitBeginRequest(element);
            }
            Thread.Sleep(pause);
        }

        /// <summary>
        /// Write into the HTML element (usually a text input) with the given name
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="name">HTML name attribute of the element to click on</param>
        /// <param name="text">Text to write</param>
        public static void Write(this ISeleniumBase inst, string name, string text, int index = 0)
        {
            var textinput = GetHTMLElementByName(inst, name, index);
            textinput.Clear();  // has GetAttribute, but no SetAttribute as SHDocVw.InternetExplorer
            textinput.SendKeys(text);
        }

        /// <summary>
        /// Write into the HTML element (usually a text input) with the given clientID
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="clientId">HTML id attribute of the element to click on</param>
        /// <param name="text">Text to write</param>
        public static void WriteID(this ISeleniumBase inst, string clientId, string text)
        {
            var textinput = GetHTMLElementById(inst, clientId);
            textinput.Clear();
            textinput.SendKeys(text);
        }

        /// <summary>
        /// Select the item with the given value from the input element
        /// collection with the given name and wait for the response when
        /// expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="name">HTML name attribute of the element to click on</param>
        /// <param name="value">value of the item to click on</param>
        /// <param name="expectPostBack">Whether to expect a js-triggered server request from the click</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Select(this ISeleniumBase inst, string name, string value, bool expectPostBack = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var list = GetHTMLElemensByName(inst, name);
            if (list == null)
            {
                throw new Exception(String.Format("No HTML input elements with name = found", name));
            }
            for (int idx = 0; idx <= list.Count; idx++)
            {
                if (idx == list.Count)
                {
                    throw new Exception(String.Format("HTML input element '{0}': value '{1}' not found", name, value));
                }
                else if (list[idx].GetAttribute("value") == value)
                {
                    ClickName(inst, name, idx, expectRequest: expectPostBack, samePage: true, expectedStatusCode: expectedStatusCode,
                            delay: delay, pause: pause);
                    if (expectPostBack)
                    {
                        var _ = new WebDriverWait(inst.driver, TimeSpan.FromSeconds(RequestTimeout))
                                    .Until(drv => drv.FindElements(By.Name(name))[idx].Selected);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Get the element with the given clientID
        /// </summary>
        /// <param name="clientID">ClientID resp. HTML id attribute of the element</param>
        /// <returns></returns>
        public static IWebElement GetHTMLElementById(this ISeleniumBase inst, string clientID)
        {
            var element = inst.driver.FindElement(By.Id(clientID));
            if (element == null)
            {
                throw new Exception(String.Format("HTML element with ClientID='{0}' not found", clientID));
            }
            else
            {
                return element;
            }
        }

        /// <summary>
        /// Get the element with the given name at the given index
        /// </summary>
        /// <param name="name">name attribute of the element</param>
        /// <param name="index">index of the element collection with that name, defaults to 0</param>
        /// <returns></returns>
        public static IWebElement GetHTMLElementByName(this ISeleniumBase inst, string name, int index = 0)
        {
            var elements = inst.driver.FindElements(By.Name(name));
            if (elements.Count <= index)
            {
                throw new ArgumentException(String.Format(
                    "HTML input element with name='{0}' at index {1} not found", name, index));
            }
            else
            {
                return elements[index];
            }
        }

        /// <summary>
        /// Get all input elements with the given name
        /// </summary>
        /// <param name="name">name attribute of the element</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> GetHTMLElemensByName(this ISeleniumBase inst, string name)
        {
            var elements = inst.driver.FindElements(By.Name(name));
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
        /// Explicitly wait for the element to disappear by polling
        /// its visibility until the reference has become stale
        /// </summary>
        /// <param name="element"></param>
        private static void AwaitBeginRequest(IWebElement element)
        {
            const int POLL_INTERVAL_MILLISECONDS = 20;  // > system quantum
            bool isPostBack = false;
            for (int i = 0; i < RequestTimeout * 1000 / POLL_INTERVAL_MILLISECONDS; i++)
            {
                try
                {
                    var _ = element.Displayed;
                }
#pragma warning disable CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
                catch (StaleElementReferenceException _)
#pragma warning restore CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
                {
                    isPostBack = true;
                    break;
                }
                Thread.Sleep(POLL_INTERVAL_MILLISECONDS);
            }
            if (!isPostBack)
            {
                throw new TimeoutException(String.Format("PostBack took longer than {0}s", RequestTimeout));
            }
        }

        /// <summary>
        /// Asserts the status code while blindly accepting responses in the
        /// AnyOf list, currently 304 ("Not Modified")
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        private static void AssertStatusCode(int expectedStatusCode)
        {
            Assert.That(StatusCode, Is.AnyOf(expectedStatusCode,
                (int)HttpStatusCode.NotModified,
                (int)HttpStatusCode.NotFound)); // WebSharper a href="#"-Links?
        }
    }
}