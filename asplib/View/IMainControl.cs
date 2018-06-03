/*
- * Extension methods for direct accessors and the implementation of the persistence of R within an UserControl
-*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;

using asplib.Model;


namespace asplib.View
{
    /// <summary>
    /// Extension interface for an UserControl with a reference to R
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="S"></typeparam>
    public interface IMainControl<M, F, S>
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
    {       
        /// <summary> 
        /// The central access pont made persistent accross requests
        /// </summary>
        /// 
        M Main { get; set; }
        /// <summary>
        /// The generated FSM class
        /// </summary>
        F Fsm { get; }

        /// <summary>
        /// The state of the Fsm class
        /// </summary>
        S State { get; set; }

        /// <summary>
        /// Local session storage type overriding the global config
        /// </summary>
        Storage? SessionStorage { get; set; }

        // Inherited UserControl properties
        bool Visible { get; set; }
        string ClientID { get; }
        HttpSessionState Session { get; }
        HttpRequest Request { get; }
        HttpResponse Response { get; }
        bool IsPostBack { get; }
        /// <summary>
        /// Make the protected ViewState public
        /// </summary>
        StateBag ViewState { get; }
    }


    /// <summary>
    /// Storate method for the persistency of R
    /// </summary>
    public enum Storage
    {
        /// <summary>
        /// Viewstate is the least persistent storage, cleared when navigating to the url
        /// </summary>
        Viewstate,
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
    /// Extension implementation
    /// </summary>
    public static class ControlMainExtension
    { 
        /// <summary>
        /// Type of the session storage to override AppSettings["SessionStorage"]
        /// </summary>
        public static Storage? SessionStorage { get; set; }
        /// <summary>
        /// Typeless reference to the current M Main for storage in Global.asax
        /// </summary>
        public static object CurrentMain { get; set; }
        /// <summary>
        /// Global reference to the main control of an application under test
        /// </summary>
        public static System.Web.UI.Control MainControl { get; set; }

        /// <summary>
        /// Set the local session storage type from an .ascx attribute string. Case insensitive.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M, F, S>(this IMainControl<M, F, S> controlMain, string storage)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            controlMain.SessionStorage = (Storage)Enum.Parse(typeof(Storage), storage, true);
        }

        /// <summary>
        /// Set the local session storage type from code
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M, F, S>(this IMainControl<M, F, S> controlMain, Storage storage)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            controlMain.SessionStorage = storage;
        }

        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Local session storage if set by SetStorage
        /// 2. Global config override in ControlMainExtension.SessionStorage e-g- from unit tests
        /// 3. Configured storage in key="SessionStorage" value="Database"
        /// 4. Defaults to Viewstate
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        public static Storage GetStorage<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            var storage = controlMain.SessionStorage;
            if (storage == null)
            {
                storage = SessionStorage;
            }
            if (storage == null)
            {
                var configStorage = ConfigurationManager.AppSettings["SessionStorage"];
                storage = String.IsNullOrEmpty(configStorage) ? Storage.Viewstate : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
        }


        /// <summary>
        /// To be called in Page_Load():
        /// Load the R object from the storage, propagate it to all subcontrols 
        /// and recursively hide them all below the main control.
        /// Also sets a global reference to the main control in iie for testing.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void LoadMain<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : class, new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            var storage = controlMain.GetStorage();
            controlMain.ClearIfRequested(storage);

            Guid sessionOverride;
            if (!controlMain.IsPostBack && Guid.TryParse(controlMain.Request.QueryString["session"], out sessionOverride))
            {
                controlMain.Main = Main.LoadMain<M>(sessionOverride);
            }
            else
            {
                switch (storage)
                {
                    case Storage.Viewstate:
                        controlMain.Main = (M)controlMain.ViewState[controlMain.StorageID()];
                        break;
                    case Storage.Session:
                        controlMain.Main = (M)controlMain.Session[controlMain.StorageID()];
                        break;
                    case Storage.Database:
                        var cookie = controlMain.Request.Cookies[controlMain.StorageID()];
                        if (cookie != null)
                        { 
                            Guid session;
                            if (Guid.TryParse(cookie["session"], out session))
                            {
                                Func<byte[], byte[]> filter = null;
                                var configEncrypt = ConfigurationManager.AppSettings["EncryptDatabaseStorage"];
                                var encrypt = String.IsNullOrEmpty(configEncrypt) ? false : Boolean.Parse(configEncrypt);
                                if (encrypt)
                                {
                                    var secret = controlMain.GetSecret();
                                    filter = x => Crypt.Decrypt(secret, x); // closure
                                }

                                controlMain.Main = Main.LoadMain<M>(session, filter);
                            }
                        }
                        break;
                }
            }

            if (controlMain.Main != null)
            {
                // SMC Manual Section 9
                // SMC considers the code it generates to be subservient to the application code. For this reason the SMC code does not serialize 
                // its references to the FSM context owner or property listeners.The application code after deserializing 
                // the FSM must call the Owner property setter to re - establish the application/ FSM link.
                // If the application listens for FSM state transitions, then event handlers must also be put back in place.
                controlMain.Fsm.GetType().GetProperty("Owner").SetValue(controlMain.Fsm, controlMain.Main);
            }
            else
            {
                controlMain.Main = new M();
            }
            CurrentMain = controlMain.Main;
            MainControl = (System.Web.UI.Control)controlMain;

            controlMain.PropagateMain(controlMain.Main);
            controlMain.HideAll();
        }


        /// <summary>
        /// To be called at the end of OnPreRender():
        /// Persist the in this page lifecycle stage immutable R object.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SaveMain<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : class, new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            var storage = controlMain.GetStorage();
            Trace.Assert(controlMain.Main != null, "SaveMain() without preceding LoadMain()");
            switch (storage)
            {
                case Storage.Viewstate:
                    controlMain.ViewState[controlMain.StorageID()] = controlMain.Main;
                    break;
                case Storage.Session:
                    controlMain.Session[controlMain.StorageID()] = controlMain.Main;
                    break;
                case Storage.Database:
                    Guid session = Guid.NewGuid();  // cannot exist in the database -> will get a new one on SaveMain()
                    var cookie = controlMain.Request.Cookies[controlMain.StorageID()];
                    if (cookie != null)
                    {
                        Guid.TryParse(controlMain.Request.Cookies[controlMain.StorageID()]["session"], out session);
                    }

                    var configEncrypt = ConfigurationManager.AppSettings["EncryptDatabaseStorage"];
                    var encrypt = String.IsNullOrEmpty(configEncrypt) ? false : Boolean.Parse(configEncrypt);
                    Func<byte[], byte[]> filter = null;
                    if (encrypt)
                    {
                        var secret = controlMain.GetSecret();
                        filter = x => Crypt.Encrypt(secret, x); // closure
                    }
                    session = Main.SaveMain(controlMain.Main, session, filter);

                    var configDays = ConfigurationManager.AppSettings["DatabaseStorageExpires"];
                    var days = String.IsNullOrEmpty(configDays) ? 1 : int.Parse(configDays);
                    controlMain.Response.Cookies[controlMain.StorageID()]["session"] = session.ToString();
                    controlMain.Response.Cookies[controlMain.StorageID()].Expires = DateTime.Now.AddDays(days);
                    break;
            }
        }

        /// <summary>
        /// Get the Key/IV secret from the cookies and generate the parts that don't yet exist
        /// and directly save it to the cookies collection
        /// </summary>
        /// <returns></returns>
        private static Crypt.Secret GetSecret<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : class, new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            Crypt.Secret secret;
            string keyString;
            string ivString;
            var cookie = controlMain.Request.Cookies[controlMain.StorageID()];
            if (cookie != null)
            {
                keyString = cookie["key"];
                ivString = cookie["iv"];
                if (keyString != null)
                {
                    var key = Convert.FromBase64String(keyString);
                    if (ivString != null)
                    {
                        var iv = Convert.FromBase64String(ivString);
                        secret = new Crypt.Secret(key, iv);
                    }
                    else
                    {
                        secret = Crypt.NewSecret(key);
                    }
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
            controlMain.Response.Cookies[controlMain.StorageID()]["key"] = Convert.ToBase64String(secret.Key);
            controlMain.Response.Cookies[controlMain.StorageID()]["iv"] = Convert.ToBase64String(secret.IV);
            return secret;
        }

        /// <summary>
        /// StorageID-String unique to the control instance to store/retrieve/clear the M
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        private static string StorageID<M, F, S>(this IMainControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            return controlMain.ClientID + "_Main";
        }


        /// <summary>
        /// Hook to clear the storage for that control with ?clear=[True|False]&endresponse=[True|False]
        /// ViewState is reset anyway on GET requests, therefore NOP in that case.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        private static void ClearIfRequested<M, F, S>(this IMainControl<M, F, S> controlMain, Storage? storage)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            if (!controlMain.IsPostBack)
            {
                bool clear = false;
                bool.TryParse(controlMain.Request.QueryString["clear"], out clear);
                bool endresponse = false;
                bool.TryParse(controlMain.Request.QueryString["endresponse"], out endresponse);

                if (clear)
                {
                    switch (storage)
                    {
                        case Storage.Viewstate:
                            break;
                        case Storage.Session:
                            controlMain.Session.Remove(controlMain.StorageID());
                            break;
                        case Storage.Database:
                            // delete from database and expire the cookie
                            var cookie = controlMain.Request.Cookies[controlMain.StorageID()];
                            if (cookie != null)
                            {
                                Guid session;
                                Guid.TryParse(controlMain.Request.Cookies[controlMain.StorageID()]["session"], out session);
                                using (var db = new ASP_DBEntities())
                                {
                                    var sql = @"
                                        DELETE FROM Main
                                        WHERE session = @session
                                    ";
                                    var param = new SqlParameter("session", session);
                                    db.Database.ExecuteSqlCommand(sql, param);
                                }
                                controlMain.Request.Cookies[controlMain.StorageID()].Expires = DateTime.Now.AddDays(-1);
                            }
                            break;
                        default:
                            throw new NotImplementedException(String.Format("Storage {0}", storage));
                    }
                    if (endresponse)
                    {
                        controlMain.Response.Clear();
                        controlMain.Response.End();
                    }
                }
            }
        }


        /// <summary>
        /// Recursively add a reference to the global R and to all subcontrols
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        internal static void PropagateMain<M, F, S>(this IMainControl<M, F, S> controlMain, M Main)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            controlMain.Main = Main;
            foreach (IMainControl<M, F, S> subcontrol in controlMain.Subcontrols())
            {
                subcontrol.PropagateMain(Main);
            }
        }


        /// <summary>
        /// Recursively make all contained subcontrols of type ControlMain<M> invisible
        /// </summary>
        /// <param name="control"></param>
        internal static void HideAll<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            controlMain.HideSubcontrols();
        }

        internal static void HideSubcontrols<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            foreach (IMainControl<M, F, S> subcontrol in controlMain.Subcontrols())
            {
                subcontrol.Visible = false;
                subcontrol.HideSubcontrols();
            }
        }

        /// <summary>
        /// Enumerate all contained subcontrols of type IMainControl
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        internal static IEnumerable<IMainControl<M, F, S>>Subcontrols<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            return
                from FieldInfo c in controlMain.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                where typeof(IMainControl<M, F, S>).IsAssignableFrom(c.FieldType)
                select (IMainControl<M, F, S>)c.GetValue(controlMain);
        }
    }
}
