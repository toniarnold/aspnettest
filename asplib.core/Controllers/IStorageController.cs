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
    /// Extension implementation with storage dependency
    /// </summary>
    public static class StorageControllerExtension
    {
        /// <summary>
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
            var storage = inst.SessionStorage;  // Controller instance property override
            if (storage == null)
            {
                storage = SessionStorage;       // static and global override
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
            return GetStorageID(inst.GetType().Name);
        }

        /// <summary>
        /// StorageID-String unique to store/retrieve/clear the Controller instance
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetStorageID(string typeName)
        {
            return String.Format("_CONTROLLER_{0}", typeName);
        }

        /// <summary>
        /// SessionStorageID-String unique to post/retrieve the storage type
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetSessionStorageID(this IStorageController inst)
        {
            return GetSessionStorageID(inst.GetType().Name);
        }

        /// <summary>
        /// SessionStorageID-String unique to store/retrieve/ar the storage type
        /// </summary>
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetSessionStorageID(string typeName)
        {
            return String.Format("_SESSIONSTORAGE_{0}", typeName);
        }

        /// <summary>
        /// Get whether to encrypt database storage in this precedence:
        /// 1. Encryption enforced  in appsettings.json if "EncryptDatabaseStorage": "True"
        /// 2. Global appsettings.json override in ControlStorageExtension.SessionStorage e.g. from unit tests
        /// 3. Defaults to false
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool GetEncryptDatabaseStorage(IConfigurationRoot configuration)
        {
            var encrypt = configuration.GetValue<bool>("EncryptDatabaseStorage");   // untyped
            bool encryptOverride = EncryptDatabaseStorage ?? false;
            return encrypt || encryptOverride;
        }

        /// <summary>
        /// Serializes the given controller into a byte array if it is
        /// instantiable by this StorageControllerExtension
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool TryGetBytes(object controller, out byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            if (controller is SerializableController)
            {
                bytes = ((SerializableController)controller).Serialize(filter); // shallow
                return true;
            }
            else if (controller.GetType().IsSerializable)
            {
                bytes = Serialization.Serialize(controller, filter);  // POCO Controller
                return true;
            }
            else
            {
                bytes = null;
                return false;
            }
        }

        /// <summary>
        /// Get the Key/IV secret from the cookie, generate the parts that
        /// don't yet exist. In ASP.NET Core it is no more possible to modify
        /// the cookie no more, thus GetSecret is called twice for decrypt and
        /// encrypt.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        internal static Crypt.Secret GetSecret(string password)
        {
            return GetSecret(Crypt.Key(password));
        }

        /// <summary>
        /// Get the Key/IV secret from the key byte[]
        /// </summary>
        /// <returns></returns>
        internal static Crypt.Secret GetSecret(byte[] key)
        {
            Crypt.Secret secret;
            if (key != null)
            {
                secret = Crypt.NewSecret(key);
            }
            else
            {
                secret = Crypt.NewSecret();
            }
            return secret;
        }

        /// <summary>
        /// name:value pair for the ViewStateInputTagHelper, to be used as
        /// <input viewstate="@ViewBag.ViewState" />
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static string ViewState(this IStorageController inst, Func<byte[], byte[]> filter = null)
        {
            byte[] bytes = inst.Bytes();
            return string.Format("{0}:{1}",
                            GetStorageID(inst),
                            Convert.ToBase64String((filter == null) ? bytes : filter(bytes)));
        }

        /// <summary>
        /// Returns a byte array as a serialization of the controller
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        private static byte[] Bytes(this IStorageController inst, Func<byte[], byte[]> filter = null)
        {
            byte[] bytes;
            if (!TryGetBytes(inst, out bytes, filter))
            {
                throw new ArgumentException(String.Format(
                    "{0} is neither a SerializableController nor a POCO controller",
                    inst.GetType()));
            }
            return bytes;
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
                var secret = GetSecret(key);
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
            inst.HttpContext.Session.Set(GetStorageID(inst), Bytes(inst));
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
            if (GetEncryptDatabaseStorage(inst.Configuration))
            {
                var key = (cookie["key"] != null) ? Convert.FromBase64String(cookie["key"]) : null;
                var secret = GetSecret(key);
                filter = x => Crypt.Encrypt(secret, x);
                newCookie["key"] = Convert.ToBase64String(secret.Key);
            }
            using (var db = new ASP_DBEntities())
            {
                var savedSession = db.SaveMain(inst.Bytes(filter), session);
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
    }
}