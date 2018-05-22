/*
 * Extension methods for the persistence of Main within an UserControl
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

using statemap;

using asplib.Control;


namespace asplib.View
{
    /// <summary>
    /// Extension interface for an UserControl with a reference to Main
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public interface IControlMain<M, F, S>
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
    }

    /// <summary>
    /// Extended UserControl to be inherited from in the web applications
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public abstract class ControlMain<M, F, S> : UserControl, IControlMain<M, F, S>
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
    {
        public M Main { get; set; }
        public abstract F Fsm { get; }
        public abstract S State { get; set; }

        /// <summary>
        /// Make the ViewState visible to ControlMainExtension
        /// </summary>
        internal new StateBag ViewState
        {
            get { return base.ViewState; }
        }
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
        /// To be called in Page_Load():
        /// Load the Main object from the storage, propagate it to all subcontrols 
        /// and hide then all in the main control.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void LoadMain<M>(this ControlMain<M, statemap.FSMContext, statemap.State> controlMain, 
                                       Storage storage = Storage.Viewstate) where M : new()
        {
            var key = controlMain.ClientID + "_Main";

            switch (storage)
            {
                case Storage.Session:
                    controlMain.Main = (M)controlMain.Session[key];
                    break;
                case Storage.Viewstate:
                    controlMain.Main = (M)controlMain.ViewState[key];
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
            if (controlMain.Main == null)
            {
                controlMain.Main = new M();
            }

            controlMain.PropagateMain(controlMain.Main);
            controlMain.HideAll();
        }


        /// <summary>
        /// To be called at the end of OnPreRender():
        /// Persist the in this page lifecycle stage immutable Main object.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SaveMain<M>(this ControlMain<M, statemap.FSMContext, statemap.State> controlMain,
                                       Storage storage = Storage.Viewstate) where M : new()
        {
            var key = controlMain.ClientID + "_Main";

            switch (storage)
            {
                case Storage.Session:
                    controlMain.Session[key] = controlMain.Main;
                    break;
                case Storage.Viewstate:
                    controlMain.ViewState[key] = controlMain.Main;
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
        /// Recursively add a reference to the global Main to all subcontrols
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        internal static void PropagateMain<M>(this ControlMain<M, statemap.FSMContext, statemap.State> controlMain, 
                                              M Main) where M : new()
        {
            controlMain.Main = Main;
            foreach (ControlMain<M, statemap.FSMContext, statemap.State> subcontrol in controlMain.Subcontrols())
            {
                subcontrol.PropagateMain(Main);
            }
        }


        /// <summary>
        /// Recursively make all contained subcontrols of type ControlMain<M> invisible
        /// </summary>
        /// <param name="control"></param>
        internal static void HideAll<M>(this ControlMain<M, statemap.FSMContext, statemap.State> controlMain) where M : new()
        {
            controlMain.HideSubcontrols();
        }

        internal static void HideSubcontrols<M>(this ControlMain<M, statemap.FSMContext, statemap.State> controlMain) where M : new()
        {
            foreach (ControlMain<M, statemap.FSMContext, statemap.State> subcontrol in controlMain.Subcontrols())
            {
                subcontrol.Visible = false;
                subcontrol.HideSubcontrols();
            }
        }

        internal static IEnumerable<ControlMain<M, statemap.FSMContext, statemap.State>>Subcontrols<M>(
            this ControlMain<M, statemap.FSMContext, statemap.State> controlMain) where M : new()
        {
            return
                from c in controlMain.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                where c.FieldType.IsSubclassOf(typeof(ControlMain<M, statemap.FSMContext, statemap.State>))
                select (ControlMain<M, statemap.FSMContext, statemap.State>)((FieldInfo)c).GetValue(controlMain);
        }
    }
}
