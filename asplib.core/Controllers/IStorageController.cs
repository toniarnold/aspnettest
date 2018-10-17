using System;
using System.Collections.Generic;
using System.Text;
using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace asplib.Controllers
{
    /// <summary>
    /// Extension interface for a Controller to make it persistent across requests.
    /// </summary>
    public interface IStorageController : IStaticController
    {
        // Required Controller members
        dynamic ViewBag { get; }
        HttpContext HttpContext { get; }

        // Additional DI members
        IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Local session storage type in the instance, overrides the global config
        /// </summary>
        Storage? SessionStorage { get; set; }
    }

    /// <summary>
    /// Extension implementation with storage dependency
    /// </summary>
    public static class ControlStorageExtension
    {
        /// <summary>
        /// Type of the session storage to override AppSettings["SessionStorage"]
        /// </summary>
        public static Storage? SessionStorage { get; set; }

        /// <summary>
        /// Encrypt the serialization byte[] when database storage is used
        /// </summary>
        public static bool? EncryptDatabaseStorage { get; set; }


        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Local session storage if set by SetStorage
        /// 2. Global config override in controlStorageExtension.SessionStorage e.g. from unit tests
        /// 3. Configured storage in key="SessionStorage" value="Database"
        /// 4. Defaults to ViewState
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static Storage GetStorage(this IStorageController inst)
        {
            var storage = inst.SessionStorage;  // Controller property
            if (storage == null)
            {
                storage = SessionStorage;   // static
            }
            if (storage == null)
            {
                var configStorage = inst.Configuration.GetValue<string>("SessionStorage");
                storage = String.IsNullOrWhiteSpace(configStorage) ? Storage.ViewState : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
        }

        /// <summary>
        /// Set the control-local session storage type. Case insensitive.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="storage"></param>
        public static void SetStorage(this IStorageController inst, string storage)
        {
            inst.SessionStorage = (Storage)Enum.Parse(typeof(Storage), storage, true);
        }

        /// <summary>
        /// Set the control-local session storage type from code
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M>(this IStorageController inst, Storage storage)
        {
            inst.SessionStorage = storage;
        }

        /// <summary>
        /// StorageID-String unique to store/retrieve/clear Controller
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetStorageID(this IStorageController inst)
        {
            return GetStorageID(inst.GetType().Name);
        }

        /// <summary>
        /// StorageID-String unique to store/retrieve/clear Controller
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetStorageID(string typeName)
        {
            return String.Format("_CONTROLLER_{0}", typeName);
        }

        /// <summary>
        /// name:value pair for the ViewStateInputTagHelper, to be used as 
        /// <input viewstate="@ViewBag.ViewState" />
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static string ViewState(this IStorageController inst)
        {
            byte[] bytes = Bytes(inst);
            return string.Format("{0}:{1}",
                            GetStorageID(inst),
                            Convert.ToBase64String(bytes));
        }

        /// <summary>
        /// Returns a byte array as a serialization of the controller
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        private static byte[] Bytes(IStorageController inst)
        {
            byte[] bytes;
            if (inst is SerializableController)
            {
                bytes = ((SerializableController)inst).Serialize(); // shallow
            }
            else if (!inst.GetType().IsSubclassOf(typeof(Controller)))
            {
                bytes = Serialization.Serialize(inst);  // POCO Controller
            }
            else
            {
                throw new ArgumentException(String.Format(
                    "{0} is neither a SerializableController nor a POCO controller", 
                    inst.GetType()));
            }

            return bytes;
        }
        

        /// <summary>
        /// Add the ViewState ipnut to the ViewBag for rendering in the view
        /// </summary>
        /// <param name="inst"></param>
        public static void AddViewState(this IStorageController inst)
        {
            inst.ViewBag.ViewState = ViewState(inst);
        }

        /// <summary>
        /// Add the serialized controller to the session.
        /// </summary>
        /// <param name="inst"></param>
        public static void AddSession(this IStorageController inst)
        {
            inst.HttpContext.Session.Set(GetStorageID(inst), Bytes(inst));
        }
    }
}
