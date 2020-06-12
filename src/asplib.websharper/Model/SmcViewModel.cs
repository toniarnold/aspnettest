using System;
using WebSharper;

namespace asplib.Model
{
    /// <summary>
    /// ViewModel for SMC task classes which exposes the State name and calls
    /// the required SetOwner() method on deserializing.
    /// The inheriting class exposes the part which is translated to JavaScript,
    /// this base class runs only on the server.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <seealso cref="asplib.Model.ViewModel{M}" />
    [JavaScript(false)]
    public abstract class SmcViewModel<TModel, TFSMContext, TState> : ViewModel<TModel>
        where TModel : class, ISmcTask<TModel, TFSMContext, TState>, new()
        where TFSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        public string State;

        [JavaScript]
        public SmcViewModel() : base()
        {
            this.State = "";
        }

        /// <summary>
        /// Recreates the Main object from the ViewState and copies members
        /// used on the client side with the overridden ExposeMembers().
        /// </summary>
        /// <param name="filter">The filter.</param>
        public override void DeserializeMain(Func<byte[], byte[]> filter = null)
        {
            this.DeserializeSmcTask(filter);
        }

        /// <summary>
        /// Calls SetOwner() after assigning main
        /// </summary>
        /// <param name="main">The main.</param>
        public override void SetMain(TModel main)
        {
            base.SetMain(main);
            var smcMain = (ISmcTask<TModel, TFSMContext, TState>)this.Main;
            smcMain.SetOwner();
        }

        /// <summary>
        /// Recreates the Main object from the ViewState and copies members
        /// used on the client side with the overridden ExposeMembers()
        /// Calls the SetOwner on the contained ISmcTask.
        /// </summary>
        /// <param name="filter">The filter.</param>
        private void DeserializeSmcTask(Func<byte[], byte[]> filter = null)
        {
            base.DeserializeMain(filter);
            var smcMain = (ISmcTask<TModel, TFSMContext, TState>)this.Main;
            smcMain.SetOwner();
        }

        /// <summary>
        /// Copies sate names and fields and properties of Main() into
        /// JavaScript fields visible to WebSharper
        /// </summary>
        public override void LoadMembers()
        {
            base.LoadMembers();
            this.State = this.Main.State.ToString();
            this.LoadStateNames();
        }

        /// <summary>
        /// Loads the concrete state names from the SMC context class.
        /// </summary>
        protected virtual void LoadStateNames()
        {
        }
    }
}