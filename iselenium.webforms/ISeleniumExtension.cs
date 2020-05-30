using asplib.View;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

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
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this ISelenium inst, string path, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetControl(inst, path);
            SeleniumExtensionBase.ClickID(inst, button.ClientID, expectRequest: expectPostBack, samePage: false,
                                            expectedStatusCode: expectedStatusCode, delay: delay, pause: pause);
        }

        /// <summary>
        /// Click the ASP.NET control element (usually a Button instance) directly and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="control">The ASP.NET control to click on</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Click(this ISelenium inst, Control control, bool expectPostBack = true, int expectedStatusCode = 200, int delay = 0, int pause = 0)
        {
            var button = GetHTMLElement(inst, control.ClientID);
            SeleniumExtensionBase.Click(inst, button, expectPostBack, expectedStatusCode, delay, pause);
        }

        /// <summary>
        /// Select the item with the given value from a ListControl and wait for the response
        /// when expectPostBack is true.
        /// </summary>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="value">value of the item to click on</param>
        /// <param name="expectPostBack">Whether to expect a server request from the click. Defaults to false</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete</param>
        public static void Select(this ISelenium inst, string path, string value, bool expectPostBack = false, int expectedStatusCode = 200, int delay = 0, int pause = 0)
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
                    SeleniumExtensionBase.ClickID(inst, itemID, expectRequest: expectPostBack, samePage: true,
                                                expectedStatusCode: expectedStatusCode, delay: delay, pause: pause);
                    break;
                }
            }
        }

        /// <summary>
        /// Write into the ASP.NET control (usually a TextBox instance) at the given path
        /// </summary>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <param name="text">Text to write</param>
        /// <param name="throttle">Time interval in milliseconds between sending chars to a text input when > 0</param>
        public static void Write(this ISelenium inst, string path, string text, int throttle = 0)
        {
            var textinput = GetHTMLElement(inst, ControlRootExtension.GetRoot(), path);
            SeleniumExtensionBase.SendKeys(textinput, text, throttle);
        }

        /// <summary>
        /// Returns the ASP.NET control instance at the given path
        /// </summary>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <returns></returns>
        public static Control GetControl(this ISelenium inst, string path)
        {
            return GetControl(inst, ControlRootExtension.GetRoot(), path);
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
        /// Navigates to the element through the given path, reads
        /// </summary>
        /// <param name="parentnode"></param>
        /// <param name="path">Member name path to the control starting at the main control</param>
        /// <returns>IHTMLElement wrapper</returns>
        private static IWebElement GetHTMLElement(this ISelenium inst, Control parentnode, string path)
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