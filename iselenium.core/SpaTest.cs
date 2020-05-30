using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Base class for Browser tests of SPA applications like WebSharper.
    /// Overrides some SeleniumExtensionBase methods with default expectPostBack: false
    /// and explicit wait for RequestTimeout seconds.
    /// </summary>
    /// <typeparam name="TWebDriver"></typeparam>
    public abstract class SpaTest<TWebDriver> : SeleniumTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// Get the HTML source of the page, wait RequestTimeout seconds for the body element to appear
        /// </summary>
        /// <param name="wait">Explicit WebDriverWait for the HTML to be rendered</param>
        /// <returns>HTML source</returns>
        public string Html(int wait = 0)
        {
            return SeleniumExtensionBase.Html(this, wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
        }

        /// Click the HTML element (usually a Button) with the given id and
        /// index, don't wait for a response as expectRequest defaults to false for an SPA,
        /// but wait RequestTimeout seconds for the element to appear.
        /// </summary>
        /// <param id="id">HTML id attribute of the element to click on</param>
        /// <param name="index">Index of the element collection with that id, defaults to 0</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page with the same HTML element</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        /// <param name="wait">Explicit WebDriverWait in seconds  for the element to appear</param>
        public void Click(string id, int index = 0,
                            bool expectRequest = false, bool samePage = false,
                            int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0)
        {
            SeleniumExtensionBase.ClickID(this, id, index,
                                            expectRequest: expectRequest, samePage: samePage,
                                            expectedStatusCode: expectedStatusCode,
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
        /// <param name="id">ClientID resp. HTML id attribute of the element</param>
        /// <param name="index">Index of the element collection with that name, defaults to 0</param>
        /// <param name="wait">Explicit WebDriverWait in seconds  for the element to appear</param>
        /// <returns></returns>
        public IWebElement GetHTMLElementById(string id, int index = 0, int wait = 0)
        {
            return SeleniumExtensionBase.GetHTMLElementById(this, id, index,
                                            wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
        }
    }
}