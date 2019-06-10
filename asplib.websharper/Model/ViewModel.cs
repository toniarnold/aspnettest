﻿using System;
using WebSharper;

namespace asplib.Model
{
    /// <summary>
    /// ViewModel for the type M. Encapsulates the ViewState to make it visible
    /// to the WebSharper client.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public abstract class ViewModel<M>
        where M : class, IStored<M>, new()
    {
        /// <summary>
        /// The serialized Main model object to be stored on the client,
        /// the object itself is marked as [NonSerialized] for WebSharper.
        /// Can as object instance only be accessed on the server side.
        /// </summary>
        [NonSerialized]
        public M Main;

        /// <summary>
        /// The Base64-encoded serialization of Main to be transferred between
        /// client and server.
        /// </summary>
        public string ViewState;

        [JavaScript]
        public ViewModel()
        {
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
        /// </summary>
        /// <param name="filter">The filter.</param>
        public virtual void DeserializeMain(Func<byte[], byte[]> filter = null)
        {
            this.Main = StorageImplementation.LoadFromViewstate(() => new M(), this.ViewState, filter);
        }
    }
}