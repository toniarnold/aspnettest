using System;
using WebSharper;

namespace asplib.Model
{
    /// <summary>
    /// ViewModel for the stored type TModel. Encapsulates the "ViewState" to make
    /// it visible to the WebSharper client.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class ViewModel<TModel>
        where TModel : class, IStored<TModel>, new()
    {
        /// <summary>
        /// The serialized Main model object to be stored on the client,
        /// the object itself is marked as [NonSerialized] for WebSharper.
        /// Can as object instance only be accessed on the server side.
        /// </summary>
        [NonSerialized]
        public TModel Main;

        /// <summary>
        /// Local session storage type in the instance, overrides the global
        /// config if written to by the Load() method and the result held for
        /// the Save() method called by the parameterless Dispose().
        /// </summary>
        [JavaScript]
        public Storage SessionStorage { get; set; }

        /// <summary>
        /// Storage string View for the Client, result of combining
        /// SessionStorage override with the global configuration.
        /// </summary>
        [JavaScript]
        public string VSessionStorage { get; set; }

        /// <summary>
        /// The Base64-encoded serialization of Main to be transferred between
        /// client and server.
        /// </summary>
        public string ViewState;

        /// <summary>
        /// For [SPAEntryPoint] in F# to distinct an uninitialized instance yet to be retrieved from the server
        /// </summary>
        [JavaScript]
        public bool IsNew { get; set; }

        /// <summary>
        /// Uninitialized instance (without FSM) required for initial Var.Create() in WebSharper
        /// </summary>
        [JavaScript]
        public ViewModel()
        {
            this.IsNew = true;
        }

        /// <summary>
        /// Copies fields and properties of Main() into serializable fields
        /// visible to WebSharper
        /// </summary>
        public virtual void LoadMembers()
        {
        }

        /// <summary>
        /// Captures members potentially modified on the client side after the
        /// initial state transition.
        /// </summary>
        public virtual void SaveMembers()
        {
        }

        /// <summary>
        /// Internally serializes the Main object into the ViewState.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void SerializeMain(Func<byte[], byte[]> filter = null)
        {
            this.ViewState = StorageImplementation.ViewState(this.Main, filter);
        }

        /// <summary>
        /// Recreates the Main object from the ViewState and copies members
        /// used on the client side with the overridden ExposeMembers().
        /// Returns a new instance if no ViewState is given
        /// </summary>
        /// <param name="filter">The filter.</param>
        public virtual void DeserializeMain(Func<byte[], byte[]> filter = null)
        {
            this.Main = StorageImplementation.LoadFromViewstate(() => new TModel(), this.ViewState, filter);
            this.IsNew = false;
        }

        /// <summary>
        /// Hook for additional setup after setting a new main.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="main">The main.</param>
        public virtual void SetMain(TModel main)
        {
            this.Main = main;
        }
    }
}