/*
 * From
 * https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/:
 * If you need to do something esoteric, you can always implement
 * IControllerActivator yourself, but I can't think of any reason that these
 * two implementations wouldn't satisfy all your requirements!
 */

using asplib.Common;
using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Reflection;

namespace asplib.Controllers
{
    public class StorageControllerActivator : IControllerActivator
    {
        private IHttpContextAccessor httpContextAccessor;

        private HttpContext HttpContext
        {
            get { return this.httpContextAccessor.HttpContext; }
        }

        private IConfigurationRoot Configuration { get; }

        public StorageControllerActivator(IHttpContextAccessor http, IConfigurationRoot configuration)
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
            object controller;
            var controllerTypeInfo = actionContext.ActionDescriptor.ControllerTypeInfo;
            var controllerType = controllerTypeInfo.AsType();
            var storageID = StorageControllerExtension.GetStorageID(controllerTypeInfo.Name);
            var sessionStorageID = StorageControllerExtension.GetSessionStorageID(controllerTypeInfo.Name);

            var storage = this.GetStorage(sessionStorageID);
            this.ClearIfRequested(storage, storageID);

            Guid sessionOverride;
            Guid session;
            byte[] bytes;

            if (this.HttpContext.Request.Method == "GET" &&
                Guid.TryParse(this.HttpContext.Request.Query["session"], out sessionOverride))
            {
                using (var db = new ASP_DBEntities())
                {
                    bytes = db.LoadMain(sessionOverride);
                    controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes);
                }
            }
            else
            {
                // ---------- Load ViewState ----------
                if (storage == Storage.ViewState &&
                    this.HttpContext.Request.Method == "POST" &&
                    this.HttpContext.Request.Form.ContainsKey(storageID))
                {
                    // input type=hidden from <input viewstate="@ViewBag.ViewState" />
                    var controllerString = this.HttpContext.Request.Form[storageID];
                    if (!String.IsNullOrEmpty(controllerString))
                    {
                        Func<byte[], byte[]> filter = null;
                        var key = this.Configuration.GetValue<string>("EncryptViewStateKey");
                        if (!String.IsNullOrEmpty(key))
                        {
                            var secret = StorageControllerExtension.GetSecret(key, storageID);
                            filter = x => Crypt.Decrypt(secret, x);
                        }
                        bytes = Convert.FromBase64String(controllerString);
                        controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes, filter);
                    }
                    else // Empty ViewState form
                    {
                        controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
                    }
                }

                // ---------- Load Session ----------
                else if (storage == Storage.Session &&
                         this.HttpContext.Session.TryGetValue(storageID, out bytes))
                {
                    controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes);
                }

                // ---------- Load Database ----------
                else if (storage == Storage.Database &&
                         Guid.TryParse(this.HttpContext.Request.Cookies[storageID].FromCookieString()["session"], out session))
                {
                    Func<byte[], byte[]> filter = null;
                    if (StorageControllerExtension.GetEncryptDatabaseStorage(Configuration))
                    {
                        var secret = StorageControllerExtension.GetSecret(
                            Convert.FromBase64String(this.HttpContext.Request.Cookies[storageID].FromCookieString()["key"]), storageID);
                        filter = x => Crypt.Decrypt(secret, x);
                    }
                    using (var db = new ASP_DBEntities())
                    {
                        bytes = db.LoadMain(session, filter);
                    }
                    controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes, filter);
                }
                else
                {
                    // ASP.NET Core implementation, no persistence, just return the new controller
                    controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
                }
            }

            return controller;
        }

        /// <summary>
        /// Counterpart to IStorageController.GetStorage() without controller instance
        /// </summary>
        /// <returns></returns>
        internal Storage GetStorage(string sessionStorageID)
        {
            Storage? storage = null;
            if (this.HttpContext.Request.Method == "POST" &&
                this.HttpContext.Request.Form.ContainsKey(sessionStorageID))
            {
                Storage postedStorage;
                if (Enum.TryParse((string)this.HttpContext.Request.Form[sessionStorageID], out postedStorage))
                {
                    storage = postedStorage;                        // Controller instance property override
                }
            }
            if (storage == null)
            {
                storage = StorageControllerExtension.SessionStorage;   // static and global override
            }
            if (storage == null)                                    // configuration or default
            {
                var configStorage = this.Configuration.GetValue<string>("SessionStorage");
                storage = String.IsNullOrWhiteSpace(configStorage) ? Storage.ViewState : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
        }

        /// <summary>
        /// Controller deserialization, either shallow (SerializableController) or deep (POCO controller)
        /// Robustly return a fresh/new instance if bytes are null.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="controllerTypeInfo"></param>
        /// <param name="controllerType"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static object DeserializeController(ControllerContext actionContext, TypeInfo controllerTypeInfo, Type controllerType,
                                                     byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            object controller;
            if (controllerTypeInfo.IsSubclassOf(typeof(SerializableController)))
            {
                // Instantiate and populate own fields with the serialized objects
                controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
                if (bytes != null)
                {
                    ((SerializableController)controller).Deserialize(bytes, filter);
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
                    controller = Activator.CreateInstance(controllerTypeInfo.GetType());
                }
            }
            else
            {
                throw new ArgumentException(String.Format(
                    "{0} is neither a SerializableController nor a POCO controller",
                    controllerType.Name));
            }

            return controller;
        }

        /// <summary>
        /// Hook to clear the storage for that control with ?clear=true
        /// ViewState is reset anyway on GET requests, therefore NOP in that case.
        /// GET-arguments:
        /// clear=[true|false]          triggers clearing the storage
        /// storage=[Session|Database]    clears the selected storage type regardless of config
        /// </summary>
        /// <param name="storageID"></param>
        internal void ClearIfRequested(Storage sessionstorage, string storageID)
        {
            bool clear = false;
            if (this.HttpContext.Request.Method == "GET" &&
                bool.TryParse(this.HttpContext.Request.Query["clear"], out clear))
            {
                Storage storage;
                Enum.TryParse<Storage>(this.HttpContext.Request.Query["storage"], true, out storage);
                if (storage == Storage.ViewState)   // no meaningful override given
                {
                    storage = sessionstorage;
                }

                switch (storage)
                {
                    case Storage.ViewState:
                        break;

                    case Storage.Session:
                        this.HttpContext.Session.Remove(storageID);
                        break;

                    case Storage.Database:
                        // delete from the database and expire the cookie
                        Guid session;
                        if (Guid.TryParse(this.HttpContext.Request.Cookies[storageID].FromCookieString()["session"], out session))
                        {
                            using (var db = new ASP_DBEntities())
                            {
                                var sql = @"
                                        DELETE FROM Main
                                        WHERE session = @session
                                    ";
                                var parameters = new object[] { new SqlParameter("session", session) };
                                db.Database.ExecuteSqlCommand(sql, parameters);
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException(String.Format("Storage {0}", storage));
                }
            }
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