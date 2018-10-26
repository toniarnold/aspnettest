using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using asplib.Model;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;


namespace asplib.Controllers
{
    /// <summary>
    /// Base class for an error page registered as app.UseExceptionHandler("/Error/Error")
    /// Intended to be used in conjunction with app.UseDeveloperExceptionPage(),
    /// therefore id adds an _ERROR_PAGE Request Header with an URL to a core dump
    /// which is displayed there.
    /// </summary>
    public class ErrorControllerBase : Controller
    {
        private ILogger logger;

        public ErrorControllerBase(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual IActionResult Error()
        {
            var error = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
                var controller = StaticControllerExtension.GetController();
                byte[] bytes;
                if (controller != null && StorageControllerExtension.TryGetBytes(controller, out bytes))
                {
                    Guid session;
                    using (var db = new ASP_DBEntities())
                    {
                        session = db.SaveMain(bytes, Guid.NewGuid()); // enforce new session, store unencrypted
                    }
                    var host = this.Request.Host.ToString();
                    var path = Regex.Replace(controller.GetType().Name, "Controller$", String.Empty);
                    var url = String.Format(@"http://{0}/{1}{2}session={3}",
                                            host, path, (path.Contains("?") ? "&" : "?"),
                                            WebUtility.UrlEncode(session.ToString()));
                    this.Request.Headers.Add("_ERROR_PAGE", url);

                    this.logger.LogError(String.Format("CORE_DUMP={0}", url));
                }
            }
            return View();        }
    }
}
