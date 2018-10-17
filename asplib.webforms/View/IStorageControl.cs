using asplib.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace asplib.View
{
    /// <summary>
    /// Extension interface for a Control to access to the LoadStorage()/SaveStorage() methods
    /// </summary>
    public interface IStorageControl<M> : IRootControl
    where M : new()
    {
        /// <summary>
        /// The central access point made persistent across requests
        /// </summary>
        M Main { get; set; }

        /// <summary>
        /// Local session storage type in the instance, overrides the global config
        /// </summary>
        Storage? SessionStorage { get; set; }

        // Inherited Control properties
        string ClientID { get; }

        bool IsPostBack { get; }
        HttpSessionState Session { get; }
        HttpRequest Request { get; }
        HttpResponse Response { get; }

        /// <summary>
        /// Make the protected ViewState public
        /// </summary>
        StateBag ViewState { get; }
    }

    /// <summary>
    /// Storage method for the persistence of M
    /// </summary>
    public enum Storage
    {
        /// <summary>
        /// ViewState is the least persistent storage, cleared when navigating to the url
        /// </summary>
        ViewState,

        /// <summary>
        /// Session is the middle persistent storage, cleared when closing the browser
        /// </summary>
        Session,

        /// <summary>
        /// Database is the most persistent storage, cleared when persistent cookies are deleted
        /// </summary>
        Database,
    }

    /// <summary>
    /// Extension implementation with storage dependency using Entity Framework 6
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
        /// Typeless reference to the current M Main for storage in Global.asax
        /// </summary>
        public static object CurrentMain { get; set; }

        /// <summary>
        /// To be called in Page_Load():
        /// Load the R object from the storage, propagate it to all sub-controls
        /// and recursively hide them all below the main control.
        /// Also sets a global reference to the main control in IIE for testing.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        public static void LoadMain<M>(this IStorageControl<M> controlStorage)
        where M : class, new()
        {
            controlStorage.ClearIfRequested();

            Guid sessionOverride;
            if (!controlStorage.IsPostBack && Guid.TryParse(controlStorage.Request.QueryString["session"], out sessionOverride))
            {
                controlStorage.Main = Main.LoadMain<M>(sessionOverride);
            }
            else
            {
                switch (controlStorage.GetStorage())
                {
                    case Storage.ViewState:
                        controlStorage.Main = (M)controlStorage.ViewState[controlStorage.StorageID()];
                        break;

                    case Storage.Session:
                        controlStorage.Main = (M)controlStorage.Session[controlStorage.StorageID()];
                        break;

                    case Storage.Database:
                        var cookie = controlStorage.Request.Cookies[controlStorage.StorageID()];
                        if (cookie != null)
                        {
                            Guid session;
                            if (Guid.TryParse(cookie["session"], out session))
                            {
                                Func<byte[], byte[]> filter = null;
                                if (controlStorage.GetEncryptDatabaseStorage())
                                {
                                    filter = x => Crypt.Decrypt(controlStorage.GetSecret(), x); // closure
                                }
                                controlStorage.Main = Main.LoadMain<M>(session, filter);
                            }
                        }
                        break;
                }
            }

            if (controlStorage.Main == null)
            {
                controlStorage.Main = new M();
            }
            CurrentMain = controlStorage.Main;
            ControlRootExtension.SetRoot(controlStorage);

            controlStorage.PropagateMain(controlStorage.Main);
        }

        /// <summary>
        /// To be called at the end of OnPreRender():
        /// Persist the in this page life-cycle stage immutable Main object.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        public static void SaveMain<M>(this IStorageControl<M> controlStorage)
            where M : class, new()
        {
            var storage = controlStorage.GetStorage();
            Trace.Assert(controlStorage.Main != null, "SaveMain() without preceding LoadMain()");
            switch (storage)
            {
                case Storage.ViewState:
                    controlStorage.ViewState[controlStorage.StorageID()] = controlStorage.Main;
                    break;

                case Storage.Session:
                    controlStorage.Session[controlStorage.StorageID()] = controlStorage.Main;
                    break;

                case Storage.Database:
                    Guid session = Guid.NewGuid();  // cannot exist in the database -> will get a new one on SaveMain()
                    var cookie = controlStorage.Request.Cookies[controlStorage.StorageID()];
                    if (cookie != null)
                    {
                        Guid.TryParse(controlStorage.Request.Cookies[controlStorage.StorageID()]["session"], out session);
                    }

                    Func<byte[], byte[]> filter = null;
                    if (controlStorage.GetEncryptDatabaseStorage())
                    {
                        filter = x => Crypt.Encrypt(controlStorage.GetSecret(), x); // closure
                    }
                    session = Main.SaveMain(controlStorage.Main, session, filter);

                    var configDays = ConfigurationManager.AppSettings["DatabaseStorageExpires"];
                    var days = String.IsNullOrWhiteSpace(configDays) ? 1 : int.Parse(configDays);
                    controlStorage.Response.Cookies[controlStorage.StorageID()]["session"] = session.ToString();
                    controlStorage.Response.Cookies[controlStorage.StorageID()].Expires = DateTime.Now.AddDays(days);
                    break;
            }
        }

        /// <summary>
        /// Set the control-local session storage type from an .ascx attribute string. Case insensitive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlStorage"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M>(this IStorageControl<M> controlStorage, string storage)
        where M : new()
        {
            controlStorage.SessionStorage = (Storage)Enum.Parse(typeof(Storage), storage, true);
        }

        /// <summary>
        /// Set the control-local session storage type from code
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlStorage"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M>(this IStorageControl<M> controlStorage, Storage storage)
        where M : new()
        {
            controlStorage.SessionStorage = storage;
        }

        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Local session storage if set by SetStorage
        /// 2. Global config override in controlStorageExtension.SessionStorage e.g. from unit tests
        /// 3. Configured storage in key="SessionStorage" value="Database"
        /// 4. Defaults to ViewState
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        /// <returns></returns>
        public static Storage GetStorage<M>(this IStorageControl<M> controlStorage)
        where M : new()
        {
            var storage = controlStorage.SessionStorage;    // Web Control property
            if (storage == null)
            {
                storage = SessionStorage;   // static
            }
            if (storage == null)
            {
                var configStorage = ConfigurationManager.AppSettings["SessionStorage"];
                storage = String.IsNullOrWhiteSpace(configStorage) ? Storage.ViewState : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
        }

        /// <summary>
        /// Get whether to encrypt database storage in this precedence:
        /// 1. Encryption enforced  in Web.config if key="EncryptDatabaseStorage" value="True"
        /// 2. Global Web.config override in controlStorageExtension.SessionStorage e.g. from unit tests
        /// 3. Defaults to false
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        /// <returns></returns>
        public static bool GetEncryptDatabaseStorage<M>(this IStorageControl<M> controlStorage)
        where M : new()
        {
            var encryptConfig = ConfigurationManager.AppSettings["EncryptDatabaseStorage"];
            bool encrypt = String.IsNullOrWhiteSpace(encryptConfig) ? false : Boolean.Parse(encryptConfig);
            bool encryptOverride = EncryptDatabaseStorage ?? false;
            return encrypt || encryptOverride;
        }

        /// <summary>
        /// Predefined event handler for the ShareButton to respond with an
        /// SQL INSERT script for the current Main, just add the button like this:
        /// <asplib:ShareButton ID="shareButton" runat="server"
        ///    OnServerClick="shareButton_Click" />
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void shareButton_Click<M>(this IStorageControl<M> controlStorage, object sender, EventArgs e)
        where M : class, new()
        {
            var mainRow = new Main();
            mainRow.SetInstance(controlStorage.Main); // serialize without filter
            controlStorage.Response.Clear();
            controlStorage.Response.AddHeader("Content-Type", "application/sql");
            controlStorage.Response.Write(mainRow.InsertSQL());
            controlStorage.Response.End();
        }

        /// <summary>
        /// Get the Key/IV secret from the cookies and generate the parts that don't yet exist
        /// and directly save it to the cookies collection
        /// </summary>
        /// <returns></returns>
        internal static Crypt.Secret GetSecret<M>(this IStorageControl<M> controlStorage)
        where M : class, new()
        {
            Crypt.Secret secret;
            string keyString;
            var cookie = controlStorage.Request.Cookies[controlStorage.StorageID()];
            if (cookie != null)
            {
                keyString = cookie["key"];
                if (keyString != null)
                {
                    var key = Convert.FromBase64String(keyString);
                    secret = Crypt.NewSecret(key);
                }
                else
                {
                    secret = Crypt.NewSecret();
                }
            }
            else
            {
                secret = Crypt.NewSecret();
            }
            controlStorage.Response.Cookies[controlStorage.StorageID()]["key"] = Convert.ToBase64String(secret.Key);
            return secret;
        }

        /// <summary>
        /// Hook to clear the storage for that control with ?clear=true
        /// ViewState is reset anyway on GET requests, therefore NOP in that casefa
        /// GET-arguments:
        /// clear=[true|false]  triggers clearing the storage
        /// endresponse=[true|false]    whether the page at the given URL
        /// storage=[ViewState|Session|Database]    clears the selected storage type regardless off config
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        /// <returns></returns>
        internal static void ClearIfRequested<M>(this IStorageControl<M> controlStorage)
        where M : new()
        {
            if (!controlStorage.IsPostBack)
            {
                bool clear = false;
                bool.TryParse(controlStorage.Request.QueryString["clear"], out clear);

                if (clear)
                {
                    Storage storage;
                    Enum.TryParse<Storage>(controlStorage.Request.QueryString["storage"], true, out storage);
                    if (storage == Storage.ViewState)   // no meaningful override given
                    {
                        storage = controlStorage.GetStorage();
                    }

                    bool endresponse = false;
                    bool.TryParse(controlStorage.Request.QueryString["endresponse"], out endresponse);

                    switch (storage)
                    {
                        case Storage.ViewState:
                            break;

                        case Storage.Session:
                            controlStorage.Session.Remove(controlStorage.StorageID());
                            break;

                        case Storage.Database:
                            // delete from the database and expire the cookie
                            var cookie = controlStorage.Request.Cookies[controlStorage.StorageID()];
                            if (cookie != null)
                            {
                                Guid session;
                                Guid.TryParse(controlStorage.Request.Cookies[controlStorage.StorageID()]["session"], out session);
                                using (var db = new ASP_DBEntities())
                                {
                                    var sql = @"
                                        DELETE FROM Main
                                        WHERE session = @session
                                    ";
                                    var param = new SqlParameter("session", session);
                                    db.Database.ExecuteSqlCommand(sql, param);
                                }
                                controlStorage.Request.Cookies[controlStorage.StorageID()].Expires = DateTime.Now.AddDays(-1);
                            }
                            break;

                        default:
                            throw new NotImplementedException(String.Format("Storage {0}", storage));
                    }
                    if (endresponse)
                    {
                        controlStorage.Response.Clear();
                        controlStorage.Response.End();
                    }
                }
            }
        }

        /// <summary>
        /// StorageID-String unique to the control instance to store/retrieve/clear the M
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        /// <returns></returns>
        internal static string StorageID<M>(this IStorageControl<M> controlStorage)
        where M : new()
        {
            return controlStorage.ClientID + "_Main";
        }

        /// <summary>
        /// Recursively add a reference to the global M and to all sub-controls
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        internal static void PropagateMain<M>(this IStorageControl<M> controlStorage, M main)
        where M : new()
        {
            controlStorage.Main = main;
            foreach (IStorageControl<M> subcontrol in controlStorage.Subcontrols())
            {
                subcontrol.PropagateMain(main);
            }
        }

        /// <summary>
        /// Enumerate all contained sub-controls of type IStorageControl
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlStorage"></param>
        /// <returns></returns>
        internal static IEnumerable<IStorageControl<M>> Subcontrols<M>(this IStorageControl<M> controlStorage)
        where M : new()
        {
            return
                from FieldInfo c in controlStorage.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                where typeof(IStorageControl<M>).IsAssignableFrom(c.FieldType)
                select (IStorageControl<M>)c.GetValue(controlStorage);
        }
    }
}