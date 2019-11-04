using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace asplib.Controllers
{
    /// <summary>
    /// Extension interface for a Controller to make it persistent across requests.
    /// </summary>
    public interface IPersistentController : IStaticController
    {
        // Required Controller members
        dynamic ViewBag { get; }

        HttpContext HttpContext { get; }

        // Additional DI members
        IConfiguration Configuration { get; }

        /// <summary>
        /// Local session storage type in the instance, overrides the global
        /// config if not null
        /// </summary>
        Storage? SessionStorage { get; set; }

        /// <summary>
        /// ViewModel used for the current View action method
        /// </summary>
        object Model { get; }

        /// <summary>
        /// dbo.Main.session when loaded from the Database
        /// </summary>
        Guid? Session { get; set; }
    }

    /// <summary>
    /// Storage Extension implementation with ASP.NET MVC Controller dependency
    /// </summary>
    public static class PersistentControllerExtension
    {
        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Local session storage if set by SetStorage
        /// 2. Global config override in controlStorageExtension.SessionStorage e.g. from unit tests
        /// 3. Configured storage in key="SessionStorage" value="Database"
        /// 4. Defaults to ViewState
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static Storage GetStorage(this IPersistentController inst)
        {
            var storage = inst.SessionStorage;  // Controller instance property override
            if (storage == null)
            {
                storage = StorageImplementation.SessionStorage;       // static and global override
            }
            if (storage == null)                // configuration or default
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
        public static void SetStorage(this IPersistentController inst, string storage)
        {
            inst.SessionStorage = (Storage)Enum.Parse(typeof(Storage), storage, true);
        }

        /// <summary>
        /// Set the control-local session storage type from code
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M>(this IPersistentController inst, Storage storage)
        {
            inst.SessionStorage = storage;
        }

        /// <summary>
        /// StorageID-String unique to store/retrieve/clear the Controller instance
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetStorageID(this IPersistentController inst)
        {
            return StorageImplementation.GetStorageID(inst.GetType().Name);
        }

        /// <summary>
        /// SessionStorageID-String unique to post/retrieve the storage type
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetSessionStorageID(this IPersistentController inst)
        {
            return StorageImplementation.GetSessionStorageID(inst.GetType().Name);
        }

        /// <summary>
        /// Add the ViewState input to the ViewBag for rendering in the view
        /// </summary>
        /// <param name="inst"></param>
        public static void SaveViewState(this IPersistentController inst)
        {
            inst.ViewBag.SessionStorage = ViewBagSessionStorage(inst, Storage.ViewState);
            var filter = StorageImplementation.EncryptViewState(inst.Configuration);
            inst.ViewBag.ViewState = ViewState(inst, filter);
        }

        /// <summary>
        /// Add the serialized controller to the session and the storage type to the ViewBag
        /// to be posted such that PersistentControllerActivator can read it.
        /// </summary>
        /// <param name="inst"></param>
        public static void SaveSession(this IPersistentController inst)
        {
            inst.ViewBag.SessionStorage = ViewBagSessionStorage(inst, Storage.Session);
            inst.HttpContext.Session.Set(GetStorageID(inst), StorageImplementation.Bytes(inst));
        }

        /// <summary>
        /// Store the serialized controller to the database. Reference to it is
        /// kept in a persistent cookie.
        /// </summary>
        /// <param name="inst"></param>
        public static void SaveDatabase(this IPersistentController inst)
        {
            inst.ViewBag.SessionStorage = ViewBagSessionStorage(inst, Storage.Database);
            StorageImplementation.SaveDatabase(inst.Configuration, inst.HttpContext, inst);
        }

        public static void SaveHeader(this IPersistentController inst)
        {
            var filter = StorageImplementation.EncryptViewState(inst.Configuration);
            inst.HttpContext.Response.Headers[StorageImplementation.HeaderName] = StorageImplementation.ViewState(inst, filter);
        }

        /// <summary>
        /// name:value pair for the ViewStateInputTagHelper, to be used as
        /// <input viewstate="@ViewBag.ViewState" />
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static string ViewState(this IPersistentController inst, Func<byte[], byte[]> filter = null)
        {
            return string.Format("{0}:{1}",
                            GetStorageID(inst),
                            StorageImplementation.ViewState(inst, filter));
        }

        /// <summary>
        /// name:value pair for the SessionStorageInputTagHelper, to be used as
        /// <input viewstate="@ViewBag.ViewState" />
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static string ViewBagSessionStorage(this IPersistentController inst, Storage sessionstorage)
        {
            return string.Format("{0}:{1}",
                            GetSessionStorageID(inst),
                            sessionstorage);
        }
    }
}