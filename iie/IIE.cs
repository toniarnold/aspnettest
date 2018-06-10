using asplib.View;
using mshtml;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

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
        public const string EXCEPTION_LINK_ID = "exception-link";

        /// <summary>
        /// MainControl.Response.StatusCode after the last request
        /// </summary>
        public static int StatusCode { get; set; }

        /// <summary>
        /// Port of the web development server to send callback HTTP requests to.
        /// </summary>
        public static int Port { get; set; }

        // Configuration
        private static int millisecondsTimeout =
            String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["RequestTimeout"]) ? 1000 :
            int.Parse(ConfigurationManager.AppSettings["RequestTimeout"]) * 1000;

        // Internet explorer
        private static SHDocVw.InternetExplorer ie;

        private static AutoResetEvent mre = new AutoResetEvent(false);

        /// <summary>
        /// Start internet explorer and set up events
        /// </summary>
        /// <param name="inst"></param>
        public static void SetUpIE(this IIE inst)
        {
            Trace.Assert(ie == null, "Only one SHDocVw.InternetExplorer instance allowed");
            ie = new SHDocVw.InternetExplorer();
            ie.AddressBar = true;
            ie.Visible = true;
            ie.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentComplete);
        }

        /// <summary>
        /// Quit internet explorer
        /// </summary>
        /// <param name="inst"></param>
        public static void TearDownIE(this IIE inst)
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
        /// <param name="inst"></param>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        public static void Navigate(this IIE inst, string path, int expectedStatusCode = 200)
        {
            Trace.Assert(path.StartsWith("/"), "path must be absolute");
            NavigateURL(inst, String.Format("http://127.0.0.1:{0}{1}", Port, path), expectedStatusCode);
        }

        /// <summary>
        /// Asynchronously issue a GET request for the URL.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        internal static void NavigateURL(this IIE inst, string url, int expectedStatusCode = 200)
        {
            ie.Navigate2(url);
            mre.WaitOne(millisecondsTimeout);
            Assert.That(IEExtension.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        /// <summary>
        /// Release the lock on http requests
        /// </summary>
        /// <param name="pDisp"></param>
        /// <param name="URL"></param>
        private static void OnDocumentComplete(object pDisp, ref object URL)
        {
            mre.Set();
        }

        /// <summary>
        /// Get the HTML document body of the current document in internet explorer
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string Html(this IIE inst)
        {
            var doc = (mshtml.IHTMLDocument2)ie.Document;
            var html = doc.body.outerHTML;
            return html;
        }

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) at the given path and wait for the response
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this IIE inst, string path, int expectedStatusCode = 200, bool expectPostBack = true, int delay = 0, int pause = 0)
        {
            var button = GetElement(inst, ControlRootExtension.RootControl, path);
            Click(button, expectedStatusCode, expectPostBack, delay, pause);
        }

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) directly and wait for the response
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="clientId">Member name path to the control starting at the main control</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>        ///
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this IIE inst, Control control, int expectedStatusCode = 200, bool expectPostBack = true, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElement(inst, control.ClientID);
            Click(button, expectedStatusCode, expectPostBack, delay, pause);
        }

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) with the given clientID and wait for the response
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="clientId">Member name path to the control starting at the main control</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>        ///
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void ClickID(this IIE inst, string clientId, int expectedStatusCode = 200, bool expectPostBack = true, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElement(inst, clientId);
            Click(button, expectedStatusCode, expectPostBack, delay, pause);
        }

        public static void Select(this IIE inst, string path, string value, int expectedStatusCode = 200, bool expectPostBack = true, int delay = 0, int pause = 0)
        {
            var list = GetControl(inst, path) as ListControl;
            if (list == null)
            {
                throw new Exception(String.Format("ListControl at '{0}' not found", path));
            }
            for (int idx = 0; idx <= list.Items.Count; idx++)
            {
                if (idx == list.Items.Count)
                {
                    throw new Exception(String.Format("ListControl at '{0}': value '{1}' not found", path, value));
                }
                else if (list.Items[idx].Value == value)
                {
                    string itemID = String.Format("{0}_{1}", list.ClientID, idx);
                    ClickID(inst, itemID, expectedStatusCode, expectPostBack, delay, pause);
                    break;
                }
            }
        }

        private static void Click(IHTMLElement element, int expectedStatusCode = 200, bool expectPostBack = true, int delay = 0, int pause = 0)
        {
            Thread.Sleep(delay);
            element.click();
            if (expectPostBack)
            {
                mre.WaitOne(millisecondsTimeout);
            }
            Thread.Sleep(pause);
            Assert.That(IEExtension.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        /// <summary>
        /// Write into the ASP.NET control (usually a TextBox instance) at the given path
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="text">Text to write</param>
        public static void Write(this IIE inst, string path, string text)
        {
            var input = GetElement(inst, ControlRootExtension.RootControl, path);
            input.setAttribute("value", text);
        }

        /// <summary>
        /// Returns the ASP.NET control instance at the given path
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <returns></returns>
        public static Control GetControl(this IIE inst, string path)
        {
            return GetControl(inst, ControlRootExtension.RootControl, path);
        }

        /// <summary>
        /// Get the element with the given clientID
        /// </summary>
        /// <param name="clientID">ClientID resp. HTML id of the element</param>
        /// <returns></returns>
        public static IHTMLElement GetHTMLElement(this IIE inst, string clientID)
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
        /// Document type for accessing the DOM
        /// </summary>
        private static mshtml.IHTMLDocument3 Document
        {
            get { return (mshtml.IHTMLDocument3)ie.Document; }
        }

        /// <summary>
        /// NAvigates to the element through the given path, reads
        /// </summary>
        /// <param name="parentnode"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <returns></returns>
        private static IHTMLElement GetElement(this IIE inst, Control parentnode, string path)
        {
            if (ControlRootExtension.RootControl == null)
            {
                throw new InvalidOperationException("IE tests must run in the w3wp.exe address space");
            }
            var control = GetControl(inst, ControlRootExtension.RootControl, path);
            return GetHTMLElement(inst, control.ClientID);
        }

        /// <summary>
        /// Recursively walk down the path starting at the MainControl instance  and return
        /// the Control instance there.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Control GetControl(this IIE inst, Control parentnode, string path)
        {
            var fields = path.Split('.');
            return GetControl(inst, parentnode, fields);
        }

        private static Control GetControl(this IIE inst, Control parentnode, IEnumerable<string> fields)
        {
            var fieldname = fields.First();
            var node = (Control)parentnode.GetType().GetField(fieldname, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(parentnode);

            if (fields.Count() == 1) // Base case: return the terminal node
            {
                return node;
            }
            else // walk down the object tree
            {
                return GetControl(inst, node, fields.Skip(1));
            }
        }
    }
}