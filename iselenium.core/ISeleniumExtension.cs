namespace iselenium
{
    public interface ISelenium : ISeleniumBase
    {
    }

    public static class SeleniumExtension
    {
        /// <summary>
        /// Click the HTML element (usually a Button) with the given name and
        /// index and wait for the response when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="name">HTML name attribute of the element to click on</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page with the same HTML element</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this ISelenium inst, string name, int index = 0,
                                bool expectRequest = true, bool samePage = false,
                                int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            SeleniumExtensionBase.ClickName(inst, name, index, 
                                            expectRequest: expectRequest, samePage: samePage,
                                            expectedStatusCode: expectedStatusCode, delay: delay, pause: pause);
        }
    }
}