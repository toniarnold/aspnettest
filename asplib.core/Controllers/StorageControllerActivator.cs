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

namespace asplib.Controllers
{
    public class StorageControllerActivator : IControllerActivator
    {
        private IHttpContextAccessor Http { get; }

        public StorageControllerActivator(IHttpContextAccessor http)
        {
            Http = http;
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

            if (Http.HttpContext.Request.Method == "POST" &&
                Http.HttpContext.Request.Form.ContainsKey(storageID))
            {
                // input type=hidden from @Html.Raw(ViewBag.ViewStateInput)
                var controllerString = Http.HttpContext.Request.Form[storageID];
                var bytes = Convert.FromBase64String(controllerString);
                if (controllerTypeInfo.IsSubclassOf(typeof(SerializableController)))
                {
                    // Instantiate and populate own fields with the serialized objects
                    controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
                    ((SerializableController)controller).Deserialize(bytes);
                }
                else if (!controllerTypeInfo.IsSubclassOf(typeof(Controller)))
                {
                    // POCO Controller is directly instantiated
                    controller = Serialization.Deserialize(bytes); 
                }
                else
                {
                    throw new ArgumentException(String.Format(
                        "Found @Html.Raw(ViewBag.ViewStateInput), but {0} is neither a POCO Controller nor a subclass of SerializableController", controllerType.Name));
                }
            }
            else
            {
                // ASP.NET Core implementation, just return the controller
                controller = actionContext.HttpContext.RequestServices.GetService(controllerType);
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
