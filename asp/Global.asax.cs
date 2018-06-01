using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using iie;

using asplib.Model;
using asplib.View;


namespace asp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// If EncryptDatabaseStorage is not true:
        /// Save the last Main object (if present) and write the direkt link with the session in the URL 
        /// to reproduce the error when debugging to the yellow screen of death.
        /// Of course, in a production environment, the URL should be logged to a persistent storage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
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
                        response.Write(String.Format("<a href='{0}'>{0}</a></br>\n", url));
                        response.Write(ysod);
                        response.End();
                    }
                }
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}