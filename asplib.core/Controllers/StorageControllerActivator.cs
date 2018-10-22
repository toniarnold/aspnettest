﻿/*
 * From
 * https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/:
 * If you need to do something esoteric, you can always implement
 * IControllerActivator yourself, but I can't think of any reason that these
 * two implementations wouldn't satisfy all your requirements!
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Http;
using asplib.Model;
using asplib.Common;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;


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

            Guid sessionOverride;
            Guid session;
            byte[] bytes;

            var storage = this.GetStorage(sessionStorageID);

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
