using asplib.Common;
using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;

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

        /// <summary>
        /// ViewModel used for the current View action method
        /// </summary>
        object Model { get; }
    }

    /// <summary>
    /// Storage Extension implementation with ASP.NET MVC Controller dependency
    /// </summary>
    public static class StorageControllerExtension
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
        public static Storage GetStorage(this IStorageController inst)
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
        /// StorageID-String unique to store/retrieve/clear the Controller instance
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetStorageID(this IStorageController inst)
        {
            return StorageImplementation.GetStorageID(inst.GetType().Name);
        }

        /// <summary>
        /// SessionStorageID-String unique to post/retrieve the storage type
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetSessionStorageID(this IStorageController inst)
        {
            return StorageImplementation.GetSessionStorageID(inst.GetType().Name);
        }


        /// <summary>
        /// Add the ViewState input to the ViewBag for rendering in the view
        /// </summary>
        /// <param name="inst"></param>
        public static void SaveViewState(this IStorageController inst)
        {
            inst.ViewBag.SessionStorage = ViewBagSessionStorage(inst, Storage.ViewState);

            Func<byte[], byte[]> filter = null;
            var key = inst.Configuration.GetValue<string>("EncryptViewStateKey");
            if (!String.IsNullOrEmpty(key))
            {
                var secret = StorageImplementation.GetSecret(key);
                filter = x => Crypt.Encrypt(secret, x);
            }
            inst.ViewBag.ViewState = ViewState(inst, filter);
        }

        /// <summary>
        /// Add the serialized controller to the session and the storage type to the ViewBag
        /// to be posted such that StorageControllerActivator can read it.
        /// </summary>
        /// <param name="inst"></param>
        public static void SaveSession(this IStorageController inst)
        {
            inst.ViewBag.SessionStorage = ViewBagSessionStorage(inst, Storage.Session);
            inst.HttpContext.Session.Set(GetStorageID(inst), StorageImplementation.Bytes(inst));
        }

        /// <summary>
        /// Store the serialized controller to the database. Reference to it is
        /// kept in a persistent cookie.
        /// </summary>
        /// <param name="inst"></param>
        public static void SaveDatabase(this IStorageController inst)
        {
            inst.ViewBag.SessionStorage = ViewBagSessionStorage(inst, Storage.Database);

            Guid session = Guid.NewGuid();  // cannot exist in the database
            var newCookie = new NameValueCollection();
            var cookie = inst.HttpContext.Request.Cookies[inst.GetStorageID()].FromCookieString();
            if (cookie != null)
            {
                Guid.TryParse(cookie["session"], out session);
            }
            Func<byte[], byte[]> filter = null;
            if (StorageImplementation.GetEncryptDatabaseStorage(inst.Configuration))
            {
                var key = (cookie["key"] != null) ? Convert.FromBase64String(cookie["key"]) : null;
                var secret = StorageImplementation.GetSecret(key);
                filter = x => Crypt.Encrypt(secret, x);
                newCookie["key"] = Convert.ToBase64String(secret.Key);
            }
            using (var db = new ASP_DBEntities())
            {
                var savedSession = db.SaveMain(inst.GetType(), StorageImplementation.Bytes(inst, filter), session);
                newCookie["session"] = savedSession.ToString();
            }

            var days = inst.Configuration.GetValue<int>("DatabaseStorageExpires");
            var options = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(days),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            };
            inst.HttpContext.Response.Cookies.Append(inst.GetStorageID(), newCookie.ToCookieString(), options);
        }

        /// <summary>
        /// name:value pair for the ViewStateInputTagHelper, to be used as
        /// <input viewstate="@ViewBag.ViewState" />
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static string ViewState(this IStorageController inst, Func<byte[], byte[]> filter = null)
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
        internal static string ViewBagSessionStorage(this IStorageController inst, Storage sessionstorage)
        {
            return string.Format("{0}:{1}",
                            GetSessionStorageID(inst),
                            sessionstorage);
        }
    }
}