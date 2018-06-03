using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using asplib.Model;
using asplib.View;

namespace iie
{
    public abstract class Global : HttpApplication
    {
        /// <summary>
        /// If EncryptDatabaseStorage is not true:
        /// Save the last Main object (if present) and write the direkt link with the session in the URL 
        /// to reproduce the error when debugging to the yellow screen of death.
        /// Of course, in a production environment, the URL should be logged to a persistent storage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_Error(object sender, EventArgs e)
        {
            var configEncrypt = ConfigurationManager.AppSettings["EncryptDatabaseStorage"];
            var encrypt = String.IsNullOrEmpty(configEncrypt) ? false : Boolean.Parse(configEncrypt);
            if (!encrypt)
            {
                if (Server.GetLastError() is HttpException exception && ControlMainExtension.CurrentMain != null)
                {
                    string ysod = exception.GetHtmlErrorMessage();
                    if (ysod != null)
                    {
                        var session = Main.SaveMain(ControlMainExtension.CurrentMain, null);
                        var requestUrl = HttpContext.Current.Request.Url.ToString();
                        var url = requestUrl + (requestUrl.Contains("?") ? "&" : "?") +
                                  String.Format("session={0}", this.Server.UrlEncode(session.ToString()));
                        var response = HttpContext.Current.Response;
                        response.Clear();
                        response.StatusCode = 500;
                        response.Write(String.Format("<a id='{0}' href='{1}'>{1}</a></br>\n", IEExtension.EXCEPTION_LINK_ID, url));
                        response.Write(ysod);
                        response.End();
                    }
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
            IEExtension.StatusCode = HttpContext.Current.Response.StatusCode;
        }
    }
}
