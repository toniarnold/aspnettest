using System;
using asplib.Model;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[assembly: InternalsVisibleTo("test.core")]

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
                //max_mainid = db.Database.SqlQuery<long>(sql).FirstOrDefault(); in EF6
                using (var conn = db.Database.GetDbConnection())
                {
                    conn.Open();
                    using (var cmd = db.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT ISNULL(MAX(mainid), 0)
                            FROM Main
                            ";
                        max_mainid = (long)cmd.ExecuteScalar();
                    }
                }
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
                var parameters = new object[] { new SqlParameter("max_mainid", max_mainid) };
                db.Database.ExecuteSqlCommand(sql, parameters);
            }
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
        /// Click the HTML element (usually a Button) with the given name and
        /// index and wait for the response when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this IIE inst, string name, int index = 0, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            IEExtensionBase.ClickName(name, index, expectPostBack, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Click the HTML element (usually a Button) with the given clientID
        /// and wait for the response when expectPostBack is true.
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
        /// Write into the HTML element (usually a text input) with the given clientID
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="clientId">HTML id attribute of the element to click on</param>
        /// <param name="text">Text to write</param>
        public static void Write(this IIE inst, string name, string text, int index = 0)
        {
            IEExtensionBase.WriteName(name, text, index);
        }

        /// <summary>
        /// Write into the HTML element (usually a text input) with the given clientID
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="clientId">HTML id attribute of the element to click on</param>
        /// <param name="text">Text to write</param>
        public static void WriteID(this IIE inst, string clientId, string text)
        {
            IEExtensionBase.WriteID(clientId, text);
        }

        /// <summary>
        /// Select the item with the given value from the input element
        /// collection with the given name and wait for the response when
        /// expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="name">Member name path to the control starting at the
        /// main control</param>
        /// <param name="value">value of the item to click on</param>
        /// <param name="expectPostBack">Whether to expect a server request
        /// from the click. Defaults to false</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the
        /// response</param>
        /// <param name="delay">Optional delay time in milliseconds before
        /// clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE
        /// claims DocumentComplete</param>
        public static void Select(this IIE inst, string name, string value, bool expectPostBack = false, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var list = IEExtensionBase.GetHTMLElements(name);
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
                else if (list[idx].getAttribute("value") == value)
                {
                    Click(inst, name, idx, expectPostBack, expectedStatusCode, delay, pause);
                    break;
                }
            }
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