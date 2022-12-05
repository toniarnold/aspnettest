using asplib.Common;
using asplib.Controllers;
using asplib.Model.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace asplib.Model
{
    /// <summary>
    /// Storage implementation w/o ASP.NET MVC specific runtime dependencies
    /// used both by the PersistentControllerExtension and the StorageServer for
    /// asplib.websharper.
    /// </summary>
    public static class StorageImplementation
    {
        /// <summary>
        /// The custom header name for session storage in the header will not become "standard"
        /// </summary>
        public const string HeaderName = "X-ViewState";

        /// <summary>
        /// Globally set Session Storage, overrides config Storage
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
        /// Sets the storage to override configuration storage.
        /// A null value restores configuration.
        /// </summary>
        /// <param name="storage"></param>
        public static void SetStorage(string storage)
        {
            SessionStorage = (storage != null) ? (Storage)Enum.Parse(typeof(Storage), storage, true) : null;
        }

        /// <summary>
        /// Save the main object into the session
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="main">The main.</param>
        public static void SaveSession(IConfiguration configuration, HttpContext httpContext, object main)
        {
            var storageID = GetStorageID(main.GetType().Name);
            httpContext.Session.Set(storageID, StorageImplementation.Bytes(main));
        }

        /// <summary>
        /// Save the main object into the database and store session ID / encryption key in a cookie.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="main">The main.</param>
        public static void SaveDatabase(IConfiguration configuration, HttpContext? httpContext, object main)
        {
            if (httpContext != null)// e.g. bUnit
            {
                var storageID = GetStorageID(main.GetType().Name);
                Guid session;
                var newCookie = new NameValueCollection();
                var cookie = httpContext.Request.Cookies[storageID].FromCookieString();
                if (cookie["session"] != null)
                {
                    Guid.TryParse(cookie["session"], out session);
                }
                else
                {
                    session = Guid.NewGuid();  // cannot exist in the database
                }
                Func<byte[], byte[]>? filter = null;
                if (StorageImplementation.GetEncryptDatabaseStorage(configuration))
                {
                    var key = (cookie["key"] != null) ? Convert.FromBase64String(cookie["key"]!) : null;
                    var secret = StorageImplementation.GetSecret(key);
                    filter = x => Crypt.Encrypt(secret, x);
                    newCookie["key"] = Convert.ToBase64String(secret.Key);
                    TypeDescriptor.AddAttributes(main, new DatabaseKeyAttribute(secret.Key));
                }
                using (var db = new ASP_DBEntities())
                {
                    var savedSession = db.SaveMain(main.GetType(), StorageImplementation.Bytes(main, filter), session);
                    newCookie["session"] = savedSession.ToString();
                    TypeDescriptor.AddAttributes(main, new DatabaseSessionAttribute(savedSession));
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
        }

        /// <summary>
        /// Save the main object into the database and store session ID / encryption key as attributes
        /// in the object itself (Blazor).
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="main"></param>
        public static void SaveDatabase(IConfiguration configuration, object main)
        {
            var storageID = GetStorageID(main.GetType().Name);
            Guid session = Guid.NewGuid();  // initially cannot exist in the database
            byte[]? key = null;
            var attributes = TypeDescriptor.GetAttributes(main);
            var sessionAttribute = (DatabaseSessionAttribute?)
                (from Attribute a in attributes
                 where a is DatabaseSessionAttribute
                 select a).FirstOrDefault();
            if (sessionAttribute != null)
            {
                session = sessionAttribute.Session; // override with the session attribute programmatically set
            }
            var keyAttribute = (DatabaseKeyAttribute?)
                (from Attribute a in attributes
                 where a is DatabaseKeyAttribute
                 select a).FirstOrDefault();
            if (keyAttribute != null)
            {
                key = keyAttribute.Key;
            }
            Func<byte[], byte[]>? filter = null;
            if (StorageImplementation.GetEncryptDatabaseStorage(configuration))
            {
                var secret = StorageImplementation.GetSecret(key);
                filter = x => Crypt.Encrypt(secret, x);
            }
            using (var db = new ASP_DBEntities())
            {
                var savedSession = db.SaveMain(main.GetType(), StorageImplementation.Bytes(main, filter), session);
            }
        }

        /// <summary>
        /// Add/Replace the X-ViewState default header on the client with the
        /// new state received from the last response for the next request.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="client"></param>
        public static void SetViewStateHeader(HttpResponseMessage response, HttpClient client)
        {
            var viewState = response.Headers.GetValues(StorageImplementation.HeaderName).ToList();
            client.DefaultRequestHeaders.Remove(StorageImplementation.HeaderName);
            client.DefaultRequestHeaders.Add(StorageImplementation.HeaderName, viewState[0]);
        }

        /// <summary>
        /// Return the filter to encrypt the ViewState with if configured so.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static Func<byte[], byte[]>? EncryptViewState(IConfiguration configuration)
        {
            Func<byte[], byte[]>? filter = null;
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
        public static Func<byte[], byte[]>? DecryptViewState(IConfiguration configuration)
        {
            Func<byte[], byte[]>? filter = null;
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
        public static (byte[] bytes, Func<byte[], byte[]>? filter) ViewStateBytes(
            IConfiguration configuration, string viewstate)
        {
            var filter = DecryptViewState(configuration);
            var bytes = Convert.FromBase64String(viewstate);
            return (bytes, filter);
        }

        public static (byte[] bytes, Func<byte[], byte[]>? filter) DatabaseBytes(
            IConfiguration configuration, HttpContext httpContext, string storageID, Guid session)
        {
            byte[] bytes;
            Func<byte[], byte[]>? filter = null;
            if (GetEncryptDatabaseStorage(configuration))
            {
                var keyString = httpContext.Request.Cookies[storageID].FromCookieString()["key"];
                var key = (keyString != null) ? Convert.FromBase64String(keyString) : null;
                var secret = GetSecret(key);
                filter = x => Crypt.Decrypt(secret, x);
            }
            using (var db = new ASP_DBEntities())
            {
                bytes = db.LoadMain(session) ?? throw new ArgumentException($"No data for session={session}"); ;
            }
            return (bytes, filter);
        }

        /// <summary>
        /// Hook to clear the storage for that control with ?clear=true
        /// ViewState is reset anyway on GET requests, therefore NOP in that case.
        /// GET-arguments:
        /// clear=[true|false]          a valid bool triggers clearing the storage
        /// storage=[Session|Database]  clears the selected storage type regardless of config
        /// </summary>
        /// <param name="sessionStorage">the configured session storage</param>
        /// <param name="storageID"></param>
        internal static void ClearIfRequested(HttpContext? httpContext, Storage sessionStorage, string storageID)
        {
            if (httpContext != null && // e.g. bUnit
                httpContext.Request.Method == WebRequestMethods.Http.Get &&
                bool.TryParse(httpContext.Request.Query["clear"], out bool _))
            {
                Enum.TryParse<Storage>(httpContext.Request.Query["storage"], true, out Storage clearstorage);
                Clear(httpContext, clearstorage, sessionStorage, storageID);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="clearStorage">the storage to clear by GET argument</param>
        /// <param name="sessionStorage">the configured session storage</param>
        /// <param name="storageID"></param>
        internal static void Clear(HttpContext httpContext, Storage clearStorage, Storage sessionStorage, string storageID)
        {
            if (clearStorage == Storage.ViewState)   // no meaningful override given
            {
                clearStorage = sessionStorage;
            }
            switch (clearStorage)
            {
                case Storage.ViewState:
                    break;

                case Storage.Session:
                    httpContext.Session.Remove(storageID);
                    break;

                case Storage.Database:
                    // expire the cookie, then delete from the database
                    Guid session;
                    if (Guid.TryParse(httpContext.Request.Cookies[storageID].FromCookieString()["session"], out session))
                    {
                        httpContext.Response.Cookies.Delete(storageID);

                        using (var db = new ASP_DBEntities())
                        {
                            db.Database.ExecuteSqlInterpolated($@"
                                    DELETE FROM Main
                                    WHERE session = {session}
                                    ");
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException(String.Format(
                        "Storage {0}", clearStorage));
            }
        }

        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Local session storage if set by SetStorage
        /// 2. Global config override in controlStorageExtension.SessionStorage e.g. from unit tests
        /// 3. Configured storage in key="SessionStorage" value="Database"
        /// 4. Defaults to ViewState
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpContext"></param>
        /// <param name="sessionStorageID">Optional session-local storage field
        /// for POST requests (not applicable for WebSharper SPA)</param>
        /// <returns></returns>
        internal static Storage GetStorage(IConfiguration configuration, HttpContext? httpContext, string? sessionStorageID = null)
        {
            Storage? storage = null;
            if (httpContext != null && // e.g. bUnit
                httpContext.Request.Method == WebRequestMethods.Http.Post &&
                httpContext.Request.HasFormContentType &&   // Exclude WebSharper's application/json
                sessionStorageID != null &&
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
        public static bool GetEncryptDatabaseStorage(IConfiguration configuration)
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
        /// <returns></returns>
        public static string InsertSQL(this object inst)
        {
            var sql = String.Empty;
            byte[]? bytes;
            if (TryGetBytes(inst, out bytes)) // serialize without filter
            {
                using (var db = new ASP_DBEntities())
                {
                    sql = db.InsertSQL(inst.GetType(), bytes!);
                }
            }
            return sql;
        }

        /// <summary>
        /// Base64 encoded serialization of the main object, to be used in
        /// ViewState HTML input fields
        /// </summary>
        /// <returns></returns>
        internal static string ViewState(object main, Func<byte[], byte[]>? filter = null)
        {
            byte[] bytes = Bytes(main);
            return Convert.ToBase64String((filter == null) ? bytes : filter(bytes));
        }

        /// <summary>
        /// Loads from ViewState.  Returns a new object if it is null.
        /// </summary>
        /// <param name="construct">Constructor function for the object if no ViewState is given</param>
        /// <param name="viewstate">The viewstate string</param>
        /// <param name="filter">optional encryption filter</param>
        /// <returns></returns>
        internal static T LoadFromViewstate<T>(Func<T> construct, string viewstate, Func<byte[], byte[]>? filter = null)
        {
            if (String.IsNullOrEmpty(viewstate))  // Initialization: return a new object
            {
                return construct();
            }
            byte[] bytes = Convert.FromBase64String(viewstate);
            return (T)Serialization.Deserialize(bytes, filter);
        }

        /// <summary>
        /// Deserializes the byte array. Returns a new object if it is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="construct">Contstructor function for the object</param>
        /// <param name="bytes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal static T LoadFromBytes<T>(Func<T> construct, byte[] bytes, Func<byte[], byte[]>? filter = null)
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
        /// <returns></returns>
        internal static byte[] Bytes(object main, Func<byte[], byte[]>? filter = null)
        {
            byte[]? bytes;
            if (!TryGetBytes(main, out bytes, filter))
            {
                throw new ArgumentException(String.Format(
                    "{0} is not a serializable object",
                    main.GetType()));
            }
            return bytes!;
        }

        /// <summary>
        /// Serializes the given controller into a byte array if it is
        /// instantiable by this PersistentControllerExtension
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public static bool TryGetBytes(object main, out byte[]? bytes, Func<byte[], byte[]>? filter = null)
        {
            if (main is PersistentController)
            {
                bytes = ((PersistentController)main).Serialize(filter); // shallow
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
        /// Get the Key/IV secret from the key byte[] or generate a new one when
        /// the key is null.
        /// </summary>
        /// <returns></returns>
        internal static Crypt.Secret GetSecret(byte[]? key)
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