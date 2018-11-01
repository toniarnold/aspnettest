using asplib.Model;
using asplib.View;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

        // Database maintenance
        internal static long max_mainid = long.MaxValue;    // guard against uninitialized WHERE mainid > @max_mainid

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
        /// [OneTimeSetUp]
        /// </summary>
        /// <param name="inst"></param>
        public static void SetUpDatabase(this IIE inst)
        {
            using (var db = new ASP_DBEntities())
            {
                var sql = @"
                    SELECT ISNULL(MAX(mainid), 0)
                    FROM Main
                    ";
                max_mainid = db.Database.SqlQuery<long>(sql).FirstOrDefault();
            }
        }

        /// <summary>
        /// [OneTimeTearDown]
        /// </summary>
        /// <param name="inst"></param>
        public static void TearDownDatabase(this IIE inst)
        {
            using (var db = new ASP_DBEntities())
            {
                var sql = @"
                    DELETE FROM Main
                    WHERE mainid > @max_mainid
                ";
                var param = new SqlParameter("max_mainid", max_mainid);
                db.Database.ExecuteSqlCommand(sql, param);
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
        public static void Navigate(this IIE inst, string path, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            IEExtensionBase.Navigate(path, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Asynchronously issue a GET request for the URL.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="url"></param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        public static void NavigateURL(this IIE inst, string url, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            IEExtensionBase.NavigateURL(url, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Setter method for the global HTTP MainControl.Response.StatusCode to check
        /// </summary>
        /// <param name="statusCode"></param>
        public static void SetStatusCode(int statusCode)
        {
            IEExtensionBase.StatusCode = statusCode;
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

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) at the given path and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this IIE inst, string path, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElement(inst, ControlRootExtension.GetRoot(), path);
            IEExtensionBase.Click(button, expectPostBack, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) directly and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="control">The ASP.NET control to click on</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this IIE inst, Control control, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElement(inst, control.ClientID);
            IEExtensionBase.Click(button, expectPostBack, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) with the given clientID and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="clientId">HTML id attribute of the element to click on</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void ClickID(this IIE inst, string clientId, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElement(inst, clientId);
            IEExtensionBase.Click(button, expectPostBack, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Select the item with the given value from a ListControl and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="value">value of the item to click on</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click. Defaults to false</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Select(this IIE inst, string path, string value, bool expectPostBack = false, int expectedStatusCode = 200, int delay = 0, int pause = 0)
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
                    ClickID(inst, itemID, expectPostBack, expectedStatusCode, delay, pause);
                    break;
                }
            }
        }

        /// <summary>
        /// Write into the ASP.NET control (usually a TextBox instance) at the given path
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="text">Text to write</param>
        public static void Write(this IIE inst, string path, string text)
        {
            var input = GetHTMLElement(inst, ControlRootExtension.GetRoot(), path);
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
            return GetControl(inst, ControlRootExtension.GetRoot(), path);
        }

        /// <summary>
        /// Get the element adapter with the given clientID
        /// </summary>
        /// <param name="clientID">ClientID resp. HTML id attribute of the element</param>
        /// <returns></returns>
        public static HTMLElement GetHTMLElement(this IIE inst, string clientID)
        {
            return IEExtensionBase.GetHTMLElement(clientID);
        }

        /// <summary>
        /// Navigates to the element through the given path, reads
        /// </summary>
        /// <param name="parentnode"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <returns></returns>
        private static HTMLElement GetHTMLElement(this IIE inst, Control parentnode, string path)
        {
            if (ControlRootExtension.GetRoot() == null)
            {
                throw new InvalidOperationException("IE tests must run in the w3wp.exe address space");
            }
            var control = GetControl(inst, ControlRootExtension.GetRoot(), path);
            return GetHTMLElement(inst, control.ClientID);
        }

        /// <summary>
        /// Recursively walk down the path starting at the MainControl instance  and return
        /// the Control instance there.
        /// </summary>
        /// <param name="path">Member name path to the control starting at the main control</param>
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