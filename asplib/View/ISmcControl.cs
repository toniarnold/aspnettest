using asplib.Model;
using System.Collections.Generic;
using System.Linq;

namespace asplib.View
{
    /// <summary>
    /// Extension interface for a Control with a reference to an FSMContext with State
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="S"></typeparam>
    public interface ISmcControl<M, F, S> : IStorageControl<M>
    where M : new()
    where F : statemap.FSMContext
    where S : statemap.State
    {
        /// <summary>
        /// The generated FSM class
        /// </summary>
        F Fsm { get; }

        /// <summary>
        /// The state of the FSM class
        /// </summary>
        S State { get; set; }

        // Inherited Control properties
        bool Visible { get; set; }
    }

    /// <summary>
    /// Extension implementation with structural SMC dependency
    /// </summary>
    public static class ControlSmcExtension
    {
        /// <summary>
        /// To be called in Page_Load():
        /// Load the R object from the storage, propagate it to all sub-controls
        /// and recursively hide them all below the main control.
        /// Also sets a global reference to the main control in IIE for testing.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void LoadMain<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : class, new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            ControlStorageExtension.LoadMain<M>(controlMain);

            // SMC Manual Section 9
            // SMC considers the code it generates to be subservient to the application code. For this reason the SMC code does not serialize
            // its references to the FSM context owner or property listeners.The application code after deserializing
            // the FSM must call the Owner property setter to re - establish the application/ FSM link.
            // If the application listens for FSM state transitions, then event handlers must also be put back in place.
            controlMain.Fsm.GetType().GetProperty("Owner").SetValue(controlMain.Fsm, controlMain.Main);

            controlMain.HideAll();
        }

        /// <summary>
        /// To be called at the end of OnPreRender():
        /// Persist the in this page life-cycle stage immutable Main object.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SaveMain<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : class, new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            ControlStorageExtension.SaveMain<M>(controlMain);
        }

        /// <summary>
        /// Set the control-local session storage type from an .ascx attribute string. Case insensitive.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M, F, S>(this ISmcControl<M, F, S> controlMain, string storage)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            ControlStorageExtension.SetStorage<M>(controlMain, storage);
        }

        /// <summary>
        /// Set the control-local session storage type from code
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <param name="storage"></param>
        public static void SetStorage<M, F, S>(this ISmcControl<M, F, S> controlMain, Storage storage)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            ControlStorageExtension.SetStorage<M>(controlMain, storage);
        }

        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Local session storage if set by SetStorage
        /// 2. Global config override in ControlStorageExtension.SessionStorage e.g. from unit tests
        /// 3. Configured storage in key="SessionStorage" value="Database"
        /// 4. Defaults to Viewstate
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        public static Storage GetStorage<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            return ControlStorageExtension.GetStorage<M>(controlMain);
        }

        /// <summary>
        /// Get whether to encrypt database storage in this precedence:
        /// 1. Global config override in ControlStorageExtension.SessionStorage e.g. from unit tests
        /// 2. Configured encryption in key="EncryptDatabaseStorage" value="True"
        /// 3. Defaults to false
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        public static bool GetEncryptDatabaseStorage<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            return ControlStorageExtension.GetEncryptDatabaseStorage<M>(controlMain);
        }

        /// <summary>
        /// Get the Key/IV secret from the cookies and generate the parts that don't yet exist
        /// and directly save it to the cookies collection
        /// </summary>
        /// <returns></returns>
        internal static Crypt.Secret GetSecret<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : class, new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            return ControlStorageExtension.GetSecret<M>(controlMain);
        }

        /// <summary>
        /// Hook to clear the storage for that control with ?clear=true
        /// ViewState is reset anyway on GET requests, therefore NOP in that casefa
        /// GET-arguments:
        /// clear=[true|false]  triggers clearing the storage
        /// endresponse=[true|false]    whether the page at the given URL
        /// storage=[Viewstate|Session|Database]    clears the selected storage type regardless off config
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        internal static void ClearIfRequested<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            ControlStorageExtension.ClearIfRequested<M>(controlMain);
        }

        /// <summary>
        /// StorageID-String unique to the control instance to store/retrieve/clear the M
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        internal static string StorageID<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            return ControlStorageExtension.StorageID<M>(controlMain);
        }

        /// <summary>
        /// Recursively add a reference to the global M and to all sub-controls
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        internal static void PropagateMain<M, F, S>(this ISmcControl<M, F, S> controlMain, M main)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            ControlStorageExtension.PropagateMain<M>(controlMain, main);
        }

        /// <summary>
        /// Enumerate all contained sub-controls of type ISmcControl
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="controlMain"></param>
        /// <returns></returns>
        internal static IEnumerable<ISmcControl<M, F, S>> Subcontrols<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            return from c in ControlStorageExtension.Subcontrols<M>(controlMain)
                   where c is ISmcControl<M, F, S>
                   select (ISmcControl<M, F, S>)c;
        }

        /// <summary>
        /// Recursively make all contained sub-controls of type ISmcControl invisible
        /// to selectively display it according to State
        /// </summary>
        /// <param name="control"></param>
        internal static void HideAll<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            controlMain.HideSubcontrols();
        }

        internal static void HideSubcontrols<M, F, S>(this ISmcControl<M, F, S> controlMain)
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
        {
            foreach (ISmcControl<M, F, S> subcontrol in controlMain.Subcontrols())
            {
                subcontrol.Visible = false;
                subcontrol.HideSubcontrols();
            }
        }
    }
}