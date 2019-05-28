using asplib.Common;
using asplib.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Net;

namespace asplib.Model
{
    /// <summary>
    /// Storage implementation w/o ASP.NET MVC specific runtime dependencies
    /// used both by the StorageControllerExtension and the StorageServer for
    /// asplib.websharper.
    /// </summary>
    public static class StorageImplementation
    {
        /// <summary>
        /// Globally set Session Storage
        /// </summary>
        public static Storage? SessionStorage { get; set; }

        /// <summary>
        /// Encrypt the serialization byte[] when database storage is used
        /// </summary>
        public static bool? EncryptDatabaseStorage { get; set; }

        /// <summary>
        /// Sets the storage to override configuration storage.
        /// A null value restores configuration.
        /// </summary>
        /// <param name="storage">The storage.</param>
        public static void SetStorage(Storage? storage)
        {
            SessionStorage = storage;
        }

        /// <summary>
        /// Saves the M main object into the session.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="main">The main.</param>
        public static void SaveSession(IConfigurationRoot configuration, HttpContext httpContext, object main)
        {
            var storageID = GetStorageID(main.GetType().Name);
            httpContext.Session.Set(storageID, StorageImplementation.Bytes(main));
        }

        /// <summary>
        /// Saves  the M main object into the database.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="main">The main.</param>
        public static void SaveDatabase(IConfigurationRoot configuration, HttpContext httpContext, object main)
        {
            var storageID = GetStorageID(main.GetType().Name);
            Guid session = Guid.NewGuid();  // cannot exist in the database
            var newCookie = new NameValueCollection();
            var cookie = httpContext.Request.Cookies[storageID].FromCookieString();
            if (cookie != null)
            {
                Guid.TryParse(cookie["session"], out session);
            }
            Func<byte[], byte[]> filter = null;
            if (StorageImplementation.GetEncryptDatabaseStorage(configuration))
            {
                var key = (cookie["key"] != null) ? Convert.FromBase64String(cookie["key"]) : null;
                var secret = StorageImplementation.GetSecret(key);
                filter = x => Crypt.Encrypt(secret, x);
                newCookie["key"] = Convert.ToBase64String(secret.Key);
            }
            using (var db = new ASP_DBEntities())
            {
                var savedSession = db.SaveMain(main.GetType(), StorageImplementation.Bytes(main, filter), session);
                newCookie["session"] = savedSession.ToString();
            }

            var days = configuration.GetValue<int>("DatabaseStorageExpires");
            var options = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(days),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            };
            httpContext.Response.Cookies.Append(storageID, newCookie.ToCookieString(), options);
        }

        /// <summary>
        /// Return the filter to encrypt the ViewState  with if configured so.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Func<byte[], byte[]> EncryptViewState(IConfigurationRoot configuration)
        {
            Func<byte[], byte[]> filter = null;
            var key = configuration.GetValue<string>("EncryptViewStateKey");
            if (!String.IsNullOrEmpty(key))
            {
                var secret = GetSecret(key);
                filter = x => Crypt.Encrypt(secret, x);
            }
            return filter;
        }

        /// <summary>
        /// Return the filter to decrypt the ViewState  with if configured so.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Func<byte[], byte[]> DecryptViewState(IConfigurationRoot configuration)
        {
            Func<byte[], byte[]> filter = null;
            var key = configuration.GetValue<string>("EncryptViewStateKey");
            if (!String.IsNullOrEmpty(key))
            {
                var secret = GetSecret(key);
                filter = x => Crypt.Decrypt(secret, x);
            }
            return filter;
        }

        /// Return the bytes and optionally the filter from a ViewState string as at tuple
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="viewstate">The Basee64-encoded ViewState string.</param>
        /// <returns></returns>
        public static (byte[] bytes, Func<byte[], byte[]> filter) ViewStateBytes(
            IConfigurationRoot configuration, string viewstate)
        {
            var filter = DecryptViewState(configuration);
            var bytes = Convert.FromBase64String(viewstate);
            return (bytes, filter);
        }

        public static (byte[] bytes, Func<byte[], byte[]> filter) DatabaseBytes(
            IConfigurationRoot configuration, HttpContext httpContext, string storageID, Guid session)
        {
            byte[] bytes;
            Func<byte[], byte[]> filter = null;
            if (GetEncryptDatabaseStorage(configuration))
            {
                var keyString = httpContext.Request.Cookies[storageID].FromCookieString()["key"];
                var key = (keyString != null) ? Convert.FromBase64String(keyString) : null;
                var secret = GetSecret(key);
                filter = x => Crypt.Decrypt(secret, x);
            }
            using (var db = new ASP_DBEntities())
            {
                bytes = db.LoadMain(session);
            }
            return (bytes, filter);
        }

        /// <summary>
        /// Hook to clear the storage for that control with ?clear=true
        /// ViewState is reset anyway on GET requests, therefore NOP in that case.
        /// GET-arguments:
        /// clear=[true|false]          triggers clearing the storage
        /// storage=[Session|Database]  clears the selected storage type regardless of config
        /// </summary>
        /// <param name="storageID"></param>
        internal static void ClearIfRequested(HttpContext httpContext, Storage sessionstorage, string storageID)
        {
            bool clear = false;
            if (httpContext.Request.Method == WebRequestMethods.Http.Get &&
                bool.TryParse(httpContext.Request.Query["clear"], out clear))
            {
                Storage storage;
                Enum.TryParse<Storage>(httpContext.Request.Query["storage"], true, out storage);
                if (storage == Storage.ViewState)   // no meaningful override given
                {
                    storage = sessionstorage;
                }

                switch (storage)
                {
                    case Storage.ViewState:
                        break;

                    case Storage.Session:
                        httpContext.Session.Remove(storageID);
                        break;

                    case Storage.Database:
                        // delete from the database and expire the cookie
                        Guid session;
                        if (Guid.TryParse(httpContext.Request.Cookies[storageID].FromCookieString()["session"], out session))
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
        /// Counterpart to IStorageController.GetStorage() without controller
        /// instance
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpContext"></param>
        /// <param name="sessionStorageID">Optional session-local storage field
        /// for POST requests (not applicable for WebSharper SPA)</param>
        /// <returns></returns>
        internal static Storage GetStorage(IConfigurationRoot configuration, HttpContext httpContext, string sessionStorageID = null)
        {
            Storage? storage = null;
            if (httpContext.Request.Method == WebRequestMethods.Http.Post &&
                httpContext.Request.HasFormContentType &&   // Exclude WebSharper's application/json
                httpContext.Request.Form.ContainsKey(sessionStorageID))
            {
                Storage postedStorage;
                if (Enum.TryParse((string)httpContext.Request.Form[sessionStorageID], out postedStorage))
                {
                    storage = postedStorage;                        // Controller instance property override
                }
            }
            if (storage == null)
            {
                storage = SessionStorage;   // static and global override
            }
            if (storage == null)                                    // configuration or default
            {
                var configStorage = configuration.GetValue<string>("SessionStorage");
                storage = String.IsNullOrWhiteSpace(configStorage) ? Storage.ViewState : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
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
        /// StorageID-String unique to store/retrieve/clear the stored
        /// object's class (the Controller in case of MVC)
        /// </summary>
        /// <param name="typeName">The name of the type that is serialized</param>
        /// <returns></returns>
        public static string GetStorageID(string typeName)
        {
            return String.Format("_STORAGEID_{0}", typeName);
        }

        /// <summary>
        /// SessionStorageID-String unique to store/retrieve/clear the storage type
        /// </summary>
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetSessionStorageID(string typeName)
        {
            return String.Format("_SESSIONSTORAGEID_{0}", typeName);
        }

        /// <summary>
        /// Render the SQL INSERT script for the serialization of the current
        /// Controller
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string InsertSQL(this object inst)
        {
            var sql = String.Empty;
            byte[] bytes;
            if (TryGetBytes(inst, out bytes)) // serialize without filter
            {
                using (var db = new ASP_DBEntities())
                {
                    sql = db.InsertSQL(inst.GetType(), bytes);
                }
            }
            return sql;
        }

        /// <summary>
        /// Base64 encoded serialization of the main object, to be used in
        /// ViewState HTML input fields
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static string ViewState(object main, Func<byte[], byte[]> filter = null)
        {
            byte[] bytes = Bytes(main);
            return Convert.ToBase64String((filter == null) ? bytes : filter(bytes));
        }

        /// <summary>
        /// Loads from viewstate.
        /// </summary>
        /// <param name="construct">Contstructor function for the object if no viewstate is given</param>
        /// <param name="viewstate">The viewstate string</param>
        /// <param name="filter">optional encryption filter</param>
        /// <returns></returns>
        internal static T LoadFromViewstate<T>(Func<T> construct, string viewstate, Func<byte[], byte[]> filter = null)
        {
            if (String.IsNullOrEmpty(viewstate))  // Initialization: return a new object
            {
                return construct();
            }
            byte[] bytes = Convert.FromBase64String(viewstate);
            return (T)Serialization.Deserialize(bytes, filter);
        }

        internal static T LoadFromBytes<T>(Func<T> construct, byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            if (bytes == null)  // Initialization: return a new object
            {
                return construct();
            }
            return (T)Serialization.Deserialize(bytes, filter);
        }

        /// <summary>
        /// Returns a byte array as a serialization of the controller
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static byte[] Bytes(object main, Func<byte[], byte[]> filter = null)
        {
            byte[] bytes;
            if (!TryGetBytes(main, out bytes, filter))
            {
                throw new ArgumentException(String.Format(
                    "{0} is not a serializable object",
                    main.GetType()));
            }
            return bytes;
        }

        /// <summary>
        /// Serializes the given controller into a byte array if it is
        /// instantiable by this StorageControllerExtension
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public static bool TryGetBytes(object main, out byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            if (main is SerializableController)
            {
                bytes = ((SerializableController)main).Serialize(filter); // shallow
                return true;
            }
            else if (main.GetType().IsSerializable)
            {
                bytes = Serialization.Serialize(main, filter);  // POCO Controller
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
    }
}