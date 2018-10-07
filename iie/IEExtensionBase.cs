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

        /// <summary>
        /// Timeout in milliseconds to wait for a HTTP response.
        /// </summary>
        public static int RequestTimeoutMS { get;  set; }

        // Internet Explorer
        private static SHDocVw.InternetExplorer ie;
        private static AutoResetEvent are = new AutoResetEvent(false);

        public static void SetUpIE()
        {
            Trace.Assert(ie == null, "Only one SHDocVw.InternetExplorer instance allowed");
            ie = new SHDocVw.InternetExplorer();
            ie.AddressBar = true;
            ie.Visible = true;
            ie.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentComplete);
        }

        public static void TearDownIE()
        {
            if (ie != null)
            {
                ie.Quit();
                ie = null;
            }
        }

        public static void Navigate(string path, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            Trace.Assert(path.StartsWith("/"), "path must be absolute");
            NavigateURL(String.Format("http://127.0.0.1:{0}{1}", Port, path), expectedStatusCode, delay, pause);
        }

        public static void NavigateURL(string url, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            Thread.Sleep(delay);
            ie.Navigate2(url);
            are.WaitOne(RequestTimeoutMS);
            Thread.Sleep(pause);
            Assert.That(StatusCode, Is.EqualTo(expectedStatusCode));
        }

        /// <summary>
        /// Release the lock on HTTP requests
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


        public static string Html()
        {
            var doc = (MSHTML.IHTMLDocument2)ie.Document;
            var html = doc.body.outerHTML;
            return html;
        }

        public static void ClickID(string clientId, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElement(clientId);
            Click(button, expectPostBack, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Get the element with the given clientID
        /// </summary>
        /// <param name="clientID">ClientID resp. HTML id attribute of the element</param>
        /// <returns></returns>
        private static IHTMLElement GetHTMLElement(string clientID)
        {
            var element = Document.getElementById(clientID);
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
        /// Get the IHTMLDocument3 Document for accessing the DOM
        /// </summary>
        private static MSHTML.IHTMLDocument3 Document
        {
            get { return (MSHTML.IHTMLDocument3)ie.Document; }
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
                are.WaitOne(RequestTimeoutMS);
            }
            Thread.Sleep(pause);
            Assert.That(StatusCode, Is.EqualTo(expectedStatusCode));
        }
    }
}
