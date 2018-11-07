﻿using asplib.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace asplib.Controllers
{
    /// <summary>
    /// Base class for an error page registered as app.UseExceptionHandler("/Error/Error")
    /// Intended to be used in conjunction with app.UseDeveloperExceptionPage(),
    /// therefore id adds a _CORE_DUMP Request Header with an URL to the core dump
    /// which is displayed there.
    /// </summary>
    public class ErrorControllerBase : Controller
    {
        private IConfigurationRoot Configuration { get; }
        private ILogger Logger { get; }

        public ErrorControllerBase(IConfigurationRoot config, ILogger logger)
        {
            this.Configuration = config;
            this.Logger = logger;
        }

        public virtual IActionResult Error()
        {
            var error = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
                var controller = StaticControllerExtension.GetController();
                byte[] bytes;
                if (controller != null &&
                    !StorageControllerExtension.GetEncryptDatabaseStorage(this.Configuration) &&
                    StorageControllerExtension.TryGetBytes(controller, out bytes))
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
                    this.Request.Headers.Add("_CORE_DUMP", url);

                    this.Logger.LogError(String.Format("_CORE_DUMP={0}", url));
                }
            }
            return View();
        }
    }
}