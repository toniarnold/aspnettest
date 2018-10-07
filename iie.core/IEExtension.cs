using System;
using System.Collections.Generic;
using System.Text;

namespace iie
{
    /// <summary>
    /// Extension marker interface for a test fixture running in Internet Explorer
    /// </summary>
    public interface IIE
    {
    }

    /// <summary>
    /// Methods for managing the Internet Explorer instance
    /// </summary>
    public static class IEExtension
    {
        /// <summary>
        /// Port of the web development server to send callback HTTP requests to.
        /// </summary>
        public static int Port
        {
            get { return IEExtensionBase.Port; }
            set { IEExtensionBase.Port = value; }
        }

        /// <summary>
        /// [OneTimeSetUp]
        /// Start Internet Explorer and set up events
        /// </summary>
        /// <param name="inst"></param>
        public static void SetUpIE(this IIE inst)
        {
            IEExtensionBase.SetUpIE();
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// Quit Internet Explorer
        /// </summary>
        public static void TearDownIE(this IIE inst)
        {
            IEExtensionBase.TearDownIE();
        }

        /// <summary>
        /// Asynchronously issue a GET request for the specified absolute path at
        /// http://127.0.0.1 as "localhost" seems to never reach OnDocumentComplete.
        /// This requires changing the binding in .\.vs\config\applicationhost.config to
        /// <binding protocol="http" bindingInformation="*:51333:127.0.0.1" />
        /// Wait for the response for AppSettings["RequestTimeout"] seconds.
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        public static void Navigate(this IIE inst, string path, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            IEExtensionBase.Navigate(path, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Asynchronously issue a GET request for the URL.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        public static void NavigateURL(this IIE inst, string url, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            IEExtensionBase.NavigateURL(url, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) with the given clientID and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="clientId">HTML id attribute of the element to click on</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void ClickID(this IIE inst, string clientId, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            IEExtensionBase.ClickID(clientId, expectPostBack, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Get the HTML document body of the current document in Internet Explorer
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string Html(this IIE inst)
        {
            return IEExtensionBase.Html();
        }
    }
}
