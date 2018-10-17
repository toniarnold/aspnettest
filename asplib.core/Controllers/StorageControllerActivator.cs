/*
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
using System.Linq;
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

        public StorageControllerActivator(IHttpContextAccessor http)
        {
            this.httpContextAccessor = http;
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
            var storageID = ControlStorageExtension.GetStorageID(controllerTypeInfo.Name);

            if (this.HttpContext.Request.Method == "POST" &&
                this.HttpContext.Request.Form.ContainsKey(storageID))
            {
                // input type=hidden from @Html.Raw(ViewBag.ViewStateInput)
                var controllerString = this.HttpContext.Request.Form[storageID];
                var bytes = Convert.FromBase64String(controllerString);
                controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes);
            }
            else if (this.HttpContext.Session.Keys.Where(k => k == storageID).Any())
            {
                // Session
                var bytes = this.HttpContext.Session.Get(storageID);
                controller = DeserializeController(actionContext, controllerTypeInfo, controllerType, bytes);
            }
            else
            {
                // ASP.NET Core implementation, , no persistence, just return the controller
                controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
            }

            return controller;
        }

        /// <summary>
        /// Controller deserialization, either shallow (SerializableController) or deep (POCO controller)
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="controllerTypeInfo"></param>
        /// <param name="controllerType"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static object DeserializeController(ControllerContext actionContext, TypeInfo controllerTypeInfo, Type controllerType, byte[] bytes)
        {
            object controller;
            if (controllerTypeInfo.IsSubclassOf(typeof(SerializableController)))
            {
                // Instantiate and populate own fields with the serialized objects
                controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
                ((SerializableController)controller).Deserialize(bytes);
            }
            else if (!controllerTypeInfo.IsSubclassOf(typeof(Controller)))
            {
                // POCO Controller is directly instantiated, replacing the instance created by the framework
                controller = Serialization.Deserialize(bytes);
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
