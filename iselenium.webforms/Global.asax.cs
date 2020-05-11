using asplib.Model;
using asplib.View;
using System;
using System.Web;

namespace iselenium
{
    public abstract class Global : HttpApplication
    {
        /// <summary>
        /// Uncaught exception handler: add a link to a core dump to the "Yellow Screen Of Death":
        /// Save the last Main object (if present) and write the direct link with the session in the URL
        /// to reproduce the error when debugging to the yellow screen of death.
        /// Of course, in a production environment, the URL should be logged to a persistent storage.
        /// If EncryptDatabaseStorage is true, the client's cookie is required to open the core dump, therefore
        /// potentially sensitive information protected by access control doesn't leak into the publicly readable [Main] table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_Error(object sender, EventArgs e)
        {
            if (Server.GetLastError() is HttpException exception &&
                !ControlStorageExtension.GetEncryptDatabaseStorage() &&
                ControlStorageExtension.CurrentMain != null)
            {
                var session = Main.SaveMain(ControlStorageExtension.CurrentMain, null);
                var requestUrl = this.Request.Url.ToString();
                var url = requestUrl + (requestUrl.Contains("?") ? "&" : "?") +
                            String.Format("session={0}", this.Server.UrlEncode(session.ToString()));
                this.Response.AppendToLog(String.Format("_CORE_DUMP={0}", url));

                string ysod = exception.GetHtmlErrorMessage();
                if (ysod != null)
                {
                    this.Response.Clear();
                    this.Response.StatusCode = 500;
                    this.Response.Write(String.Format("<a id='{0}' href='{1}'>{1}</a></br>\n", SeleniumExtension.EXCEPTION_LINK_ID, url));
                    this.Response.Write(ysod);
                    this.Response.End();
                }
            }
        }

        /// <summary>
        /// Store the Response.StatusCode for assertions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
            SeleniumExtensionBase.StatusCode = this.Response.StatusCode;
        }
    }
}