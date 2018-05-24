/*
- * Extension methods for direct accessors and the implementation of the persistence of Main within an UserControl
-*/

using System;
using System.Collections.Generic;
using System.Configuration;
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

using iie;


namespace asplib.View
{
    /// <summary>
    /// Extension interface for an UserControl with a reference to Main
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

        // Inherited UserControl properties
        bool Visible { get; set; }
        string ClientID { get; }
        HttpSessionState Session { get; }
        /// <summary>
        /// Make the protected ViewState public
        /// </summary>
        StateBag ViewState { get; }
    }


    /// <summary>
    /// Storate method for the persistency of Main
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
        /// Type of the session storage, read from AppSettings["SessionStorage"], but can be changed programmatically
        /// </summary>
        public static Storage? SessionStorage { get; set; }

        /// <summary>
        /// To be called in Page_Load():
        /// Load the Main object from the storage, propagate it to all subcontrols 
        /// and recursively hide them all below the main control.
        /// Also sets a global reference to the main control in iie for testing.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void LoadMain<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            var key = controlMain.ClientID + "_Main";

            if (SessionStorage == null)
            {
                var configStorage = ConfigurationManager.AppSettings["SessionStorage"];
                SessionStorage = String.IsNullOrEmpty(configStorage) ? Storage.Viewstate : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }

            switch (SessionStorage)
            {
                case Storage.Viewstate:
                    controlMain.Main = (M)controlMain.ViewState[key];
                    break;
                case Storage.Session:
                    controlMain.Main = (M)controlMain.Session[key];
                    break;
                case Storage.Database:
                    using (var stream = new MemoryStream())
                    {
                        //byte[] main;
                        var formattter = new BinaryFormatter();
                        controlMain.Main = (M)formattter.Deserialize(stream);
                    }

                    break;
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

            controlMain.PropagateMain(controlMain.Main);
            controlMain.HideAll();

            IEExtension.MainControl = (System.Web.UI.Control)controlMain;
        }


        /// <summary>
        /// To be called at the end of OnPreRender():
        /// Persist the in this page lifecycle stage immutable Main object.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SaveMain<M, F, S>(this IMainControl<M, F, S> controlMain)
            where M : new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            var key = controlMain.ClientID + "_Main";

            Trace.Assert(SessionStorage != null, "SaveMain() without preceding LoadMain()");
            switch (SessionStorage)
            {
                case Storage.Viewstate:
                    controlMain.ViewState[key] = controlMain.Main;
                    break;
                case Storage.Session:
                    controlMain.Session[key] = controlMain.Main;
                    break;
                case Storage.Database:
                    using (var stream = new MemoryStream())
                    {
                        var formattter = new BinaryFormatter();
                        formattter.Serialize(stream, controlMain.Main);
                        byte[] main = stream.ToArray();
                    }

                    break;
            }
        }


        /// <summary>
        /// Recursively add a reference to the global Main and to all subcontrols
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
