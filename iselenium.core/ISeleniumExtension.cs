namespace iselenium
{
    public interface ISelenium : ISeleniumBase
    {
    }

    public static class SeleniumExtension
    {
        /// <summary>
        /// Click the HTML element (usually a Button) with the given id and
        /// index and wait for the response when expectPostBack is true (default).
        /// </summary>
        /// <param id="id">HTML id attribute of the element to click on</param>
        /// <param id="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param id="samePage">Whether to expect a WebForms style PostBack to the same page with the same HTML element</param>
        /// <param id="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param id="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param id="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this ISelenium inst, string id, int index = 0,
                                bool expectRequest = true, bool samePage = false,
                                int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            SeleniumExtensionBase.ClickID(inst, id, index,
                                            expectRequest: expectRequest, samePage: samePage,
                                            expectedStatusCode: expectedStatusCode, delay: delay, pause: pause);
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
        public static void Select(this ISeleniumBase inst, string id, string value, bool expectPostBack = false,
                                    int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            SeleniumExtensionBase.SelectID(inst, id, value,
                                expectPostBack: expectPostBack,
                                expectedStatusCode: expectedStatusCode, delay: delay, pause: pause);
        }
    }
}