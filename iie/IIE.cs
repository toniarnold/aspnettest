using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

using SHDocVw;
using mshtml;


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
        /// Global reference to the main control of an application under test
        /// </summary>
        public static Control MainControl { get; set; }

        /// <summary>
        /// Port of the web development server to send callback HTTP requests to.
        /// </summary>
        public static int Port { get; set; }

        // Configuration
        private static int millisecondsTimeout =
            String.IsNullOrEmpty(ConfigurationManager.AppSettings["RequestTimeout"]) ? 1 :
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
        public static void Navigate(this IIE inst, string path)
        {
            Trace.Assert(path.StartsWith("/"), "path must be absolute");
            NavigateURL(inst, String.Format("http://127.0.0.1:{0}{1}", Port, path));
        }

        /// <summary>
        /// Asynchronously issue a GET request for the URL.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="url"></param>
        internal static void NavigateURL(this IIE inst, string url)
        {
            ie.Navigate2(url);
            mre.WaitOne(millisecondsTimeout);
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
        /// <param name="path"></param>
        public static void Click(this IIE inst, string path)
        {
            var button = GetElement(MainControl, path);
            button.click();
            mre.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Write ASP.NET control element (usually a TextBox instance) at the given path
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path"></param>
        public static void Write(this IIE inst, string path, string text)
        {
            var input = GetElement(MainControl, path);
            input.setAttribute("value", text);
        }


        /// <summary>
        /// Document type for accessing the DOM
        /// </summary>
        private static mshtml.IHTMLDocument3 Document
        {
            get { return (mshtml.IHTMLDocument3)ie.Document; }
        }

        private static IHTMLElement GetElement(Control parentnode, string path)
        {
            Trace.Assert(MainControl != null, "IE tests must run in the w3wp.exe adrdress space");
            var control = GetControl(MainControl, path);
            var element = Document.getElementById(control.ClientID);
            if (element == null)
            {
                throw new Exception(String.Format("HTML element with ClientID='{0}' not found", control.ClientID));
            }
            else
            {
                return element;
            }
        }

        /// <summary>
        /// Recursively walk down the path starting at the MainControl instance  and return
        /// the Control instance there.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Control GetControl(Control parentnode, string path)
        {
            var fields = path.Split('.');
            return GetControl(parentnode, fields);
        }

        private static Control GetControl(Control parentnode, IEnumerable<string> fields)
        {
            var fieldname = fields.First();
            var node = (Control)parentnode.GetType().GetField(fieldname, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(parentnode);

            if (fields.Count() == 1) // Base case: return the terminal node
            {
                return node;
            }
            else // walk down the object tree
            {
                return GetControl(node, fields.Skip(1));
            }
        }
    }
}
