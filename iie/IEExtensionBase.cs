using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using MSHTML;

namespace iie
{
    /// <summary>
    /// Static base class equivalent for both IEExtension classes
    /// </summary>
    public static class IEExtensionBase
    {
        /// <summary>
        /// Port of the web development server to send callback HTTP requests to.
        /// </summary>
        public static int Port { get; set; }

        /// <summary>
        /// MainControl.Response.StatusCode after the last request
        /// </summary>
        public static int StatusCode { get; set; }

        // Internet Explorer
        private static SHDocVw.InternetExplorer ie;
        private static AutoResetEvent are = new AutoResetEvent(false);
        private static int millisecondsTimeout;

        /// <summary>
        /// [OneTimeSetUp]
        /// Start Internet Explorer and set up events
        /// </summary>
        public static void SetUpIE()
        {
            Trace.Assert(ie == null, "Only one SHDocVw.InternetExplorer instance allowed");
            ie = new SHDocVw.InternetExplorer();
            ie.AddressBar = true;
            ie.Visible = true;
            ie.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentComplete);
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// Quit Internet Explorer
        /// </summary>
        public static void TearDownIE()
        {
            if (ie != null)
            {
                ie.Quit();
                ie = null;
            }
        }

        /// <summary>
        /// Asynchronously issue a GET request for the specified absolute path at
        /// http://127.0.0.1 as "localhost" seems to never reach OnDocumentComplete.
        /// This requires changing the binding in .\.vs\config\applicationhost.config to
        /// <binding protocol="http" bindingInformation="*:51333:127.0.0.1" />
        /// Wait for the response for AppSettings["RequestTimeout"] seconds.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        public static void Navigate(string path, int expectedStatusCode = 200)
        {
            Trace.Assert(path.StartsWith("/"), "path must be absolute");
            NavigateURL(String.Format("http://127.0.0.1:{0}{1}", Port, path), expectedStatusCode);
        }

        /// <summary>
        /// Asynchronously issue a GET request for the URL.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        public static void NavigateURL(string url, int expectedStatusCode = 200)
        {
            ie.Navigate2(url);
            are.WaitOne(millisecondsTimeout);
            Assert.That(StatusCode, Is.EqualTo(expectedStatusCode));
        }

        /// <summary>
        /// Release the lock on http requests
        /// </summary>
        /// <param name="pDisp"></param>
        /// <param name="URL"></param>
        private static void OnDocumentComplete(object pDisp, ref object URL)
        {
            are.Set();
        }

        /// <summary>
        /// Setter method for the global HTTP MainControl.Response.StatusCode to check
        /// </summary>
        /// <param name="statusCode"></param>
        public static void SetStatusCode(int statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Get the HTML document body of the current document in Internet Explorer
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string Html()
        {
            var doc = (MSHTML.IHTMLDocument2)ie.Document;
            var html = doc.body.outerHTML;
            return html;
        }

        /// <summary>
        /// Click on the HTML element and wait for the response when expectPostBack is true.
        /// </summary>
        /// <param name="element">The HTML element itself</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        private static void Click(IHTMLElement element, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            Thread.Sleep(delay);
            element.click();
            if (expectPostBack)
            {
                are.WaitOne(millisecondsTimeout);
            }
            Thread.Sleep(pause);
            Assert.That(StatusCode, Is.EqualTo(expectedStatusCode));
        }
    }
}
