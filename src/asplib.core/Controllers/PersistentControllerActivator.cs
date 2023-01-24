/*
 * From
 * https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/:
 * If you need to do something esoteric, you can always implement
 * IControllerActivator yourself, but I can't think of any reason that these
 * two implementations wouldn't satisfy all your requirements!
 */

using asplib.Common;
using asplib.Model;
using asplib.Model.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Reflection;

namespace asplib.Controllers
{
    public class PersistentControllerActivator : IControllerActivator
    {
        private IHttpContextAccessor httpContextAccessor;

        private HttpContext HttpContext
        {
            get
            {
                if (this.httpContextAccessor.HttpContext == null)
                {
                    throw new NullReferenceException("HttpContext not available");
                }
                return this.httpContextAccessor.HttpContext;
            }
        }

        private IConfiguration Configuration { get; }

        public PersistentControllerActivator(IHttpContextAccessor http, IConfiguration configuration)
        {
            this.httpContextAccessor = http;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Custom Controller factory method returning a serialized object if available.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public object Create(ControllerContext actionContext)
        {
            object? controller = null;
            var controllerTypeInfo = actionContext.ActionDescriptor.ControllerTypeInfo;
            var controllerType = controllerTypeInfo.AsType();
            var storageID = StorageImplementation.GetStorageID(controllerTypeInfo.Name);
            var sessionStorageID = StorageImplementation.GetSessionStorageID(controllerTypeInfo.Name);

            var storage = StorageImplementation.GetStorage(this.Configuration, this.HttpContext, sessionStorageID);
            var cleared = StorageImplementation.ClearIfRequested(this.HttpContext, storage, storageID);

            Guid sessionOverride;
            Guid session;
            byte[] bytes;
            Func<byte[], byte[]>? filter = null;

            // ---------- Direct GET request ?session= from the Database ----------
            if (this.HttpContext.Request.Method == WebRequestMethods.Http.Get &&
                Guid.TryParse(this.HttpContext.Request.Query["session"], out sessionOverride))
            {
                using (var db = new ASP_DBEntities())
                {
                    (bytes, filter) = StorageImplementation.DatabaseBytes(Configuration, HttpContext, storageID, sessionOverride);
                    controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes, filter);
                }
            }
            else
            {
                // ---------- Load from ViewState ----------
                if (storage == Storage.ViewState &&
                    this.HttpContext.Request.Method == WebRequestMethods.Http.Post &&
                    this.HttpContext.Request.Form.ContainsKey(storageID))
                {
                    // input type=hidden from <input viewstate="@ViewBag.ViewState" />
                    var controllerString = this.HttpContext.Request.Form[storageID];
                    if (!String.IsNullOrEmpty(controllerString))
                    {
                        (bytes, filter) = StorageImplementation.ViewStateBytes(this.Configuration, controllerString);
                        controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes, filter);
                    }
                }

                // ---------- Load from Session ----------
                else if (storage == Storage.Session &&
                         this.HttpContext.Session.TryGetValue(storageID, out bytes!))
                {
                    controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes);
                }

                // ---------- Load from Database ----------
                else if (storage == Storage.Database &&
                         !cleared &&    // cookie is still there, will only be cleared in the response
                         Guid.TryParse(
                             this.HttpContext.Request.Cookies[storageID].FromCookieString()["session"],
                             out session))
                {
                    (bytes, filter) = StorageImplementation.DatabaseBytes(Configuration, HttpContext, storageID, session);
                    controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes, filter);
                    ((IPersistentController)controller).Session = session;
                }

                // ---------- Load from X-ViewState Header ----------
                else if (storage == Storage.Header &&
                            this.HttpContext.Request.Headers.ContainsKey(StorageImplementation.HeaderName))
                {
                    // input type=hidden from <input viewstate="@ViewBag.ViewState" />
                    var controllerString = this.HttpContext.Request.Headers[StorageImplementation.HeaderName];
                    if (!String.IsNullOrEmpty(controllerString))
                    {
                        (bytes, filter) = StorageImplementation.ViewStateBytes(this.Configuration, controllerString);
                        controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes, filter);
                    }
                }

                if (controller == null)
                {
                    // ASP.NET Core implementation, no persistence, just return the new controller
                    controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
                    if (controller == null)
                    {
                        throw new ArgumentException($"No service registered for {controllerType}");
                    }
                }
            }
            return controller;
        }

        /// <summary>
        /// Controller deserialization, either shallow (PersistentController) or deep (POCO controller)
        /// Robustly return a fresh/new instance if bytes are null.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="controllerTypeInfo"></param>
        /// <param name="controllerType"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static object DeserializeController(ControllerContext actionContext, TypeInfo controllerTypeInfo, Type controllerType,
                                                     byte[]? bytes, Func<byte[], byte[]>? filter = null)
        {
            object? controller;
            if (controllerTypeInfo.IsSubclassOf(typeof(PersistentController)))
            {
                // Instantiate and populate own fields with the serialized objects
                controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
                if (controller == null)
                {
                    throw new ArgumentException($"No service registered for {controllerType}");
                }
                if (bytes != null)
                {
                    ((PersistentController)controller).Deserialize(bytes, filter);
                }
            }
            else if (controllerTypeInfo.IsSerializable)
            {
                // POCO Controller is directly instantiated, replacing the instance created by the framework
                if (bytes != null)
                {
                    controller = Serialization.Deserialize(bytes, filter);
                }
                else
                {
                    controller = Activator.CreateInstance(controllerType);
                }
            }
            else
            {
                throw new ArgumentException(String.Format(
                    "{0} is neither a PersistentController nor a POCO controller",
                    controllerType.Name));
            }

            return controller!;
        }

        /// <summary>
        /// Handle IDisposable Controllers
        /// </summary>
        /// <param name="context"></param>
        /// <param name="controller"></param>
        public virtual void Release(ControllerContext context, object controller)
        {
            var disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}