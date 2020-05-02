﻿using System;
using WebSharper;

namespace asplib.Model
{
    /// <summary>
    /// ViewModel for SMC task classes which exposes the State name and calls
    /// the required SetOwner() method on deserializing.
    /// The inheriting class exposes the part which is translated to JavaScript,
    /// this base class runs only on the server.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <seealso cref="asplib.Model.ViewModel{M}" />
    [JavaScript(false)]
    public abstract class SmcViewModel<M, F, S> : ViewModel<M>
        where M : class, IStored<M>, new()
        where F : statemap.FSMContext
        where S : statemap.State
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
        public override void SetMain(M main)
        {
            base.SetMain(main);
            var smcMain = (ISmcTask<M, F, S>)this.Main;
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
            var smcMain = (ISmcTask<M, F, S>)this.Main;
            smcMain.SetOwner();
        }

        /// <summary>
        /// Copies sate names and fields and properties of Main() into
        /// serializable fields visible to WebSharper
        /// </summary>
        public override void LoadMembers()
        {
            base.LoadMembers();
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