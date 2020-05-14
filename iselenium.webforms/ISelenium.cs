using asplib.View;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;

namespace iselenium
{
    /// <summary>
    /// Marked intterface for WebFormsExtension ASP.NET WebForms methods
    /// </summary>
    public interface ISelenium : ISeleniumBase
    {
    }

    public static class SeleniumExtension
    {
        /// <summary>
        /// To be used with ClickID()
        /// </summary>
        public const string EXCEPTION_LINK_ID = "exception-link";

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) at the given path and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="samePage">Whether to expect a WebForms style PostBack to the same page with the same control</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this ISelenium inst, string path,
                                bool expectRequest = true, bool samePage = false,
                                int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetControl(inst, path);
            SeleniumExtensionBase.ClickID(inst, button.ClientID, expectRequest: expectRequest, samePage: samePage,
                                            expectedStatusCode: expectedStatusCode, delay: delay, pause: pause);
        }

        /// <summary>
        /// Get the element adapter with the given clientID
        /// </summary>
        /// <param name="clientID">ClientID resp. HTML id attribute of the element</param>
        /// <returns></returns>
        public static IWebElement GetHTMLElement(this ISelenium inst, string clientID)
        {
            return SeleniumExtensionBase.GetHTMLElementById(inst, clientID);
        }

        /// <summary>
        /// Returns the ASP.NET control instance at the given path
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <returns></returns>
        public static Control GetControl(this ISelenium inst, string path)
        {
            return GetControl(inst, ControlRootExtension.GetRoot(), path);
        }

        /// <summary>
        /// Recursively walk down the path starting at the MainControl instance  and return
        /// the Control instance there.
        /// </summary>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <returns></returns>
        private static Control GetControl(this ISelenium inst, Control parentnode, string path)
        {
            var fields = path.Split('.');
            return GetControl(inst, parentnode, fields);
        }

        private static Control GetControl(this ISelenium inst, Control parentnode, IEnumerable<string> fields)
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