﻿using Microsoft.Extensions.Configuration;
using System;

namespace asplib.Controllers
{
    // SMC Manual Section 9
    // SMC considers the code it generates to be subservient to the application code. For this reason the SMC code does not serialize
    // its references to the FSM context owner or property listeners.The application code after deserializing
    // the FSM must call the Owner property setter to re - establish the application/ FSM link.
    // If the application listens for FSM state transitions, then event handlers must also be put back in place.

    /// <summary>
    /// Specialization of PersistentController with State
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="S"></typeparam>
    public class SmcController<F, S> : PersistentController
        where F : statemap.FSMContext
        where S : statemap.State
    {
        public SmcController()
        {
        }   // NUnit

        public SmcController(IConfiguration configuration) : base(configuration)
        {
            this.Construct();
        }

        protected virtual void Construct()
        {
            // Create manually due to missing constructor argument:
            var inst = (F?)Activator.CreateInstance(typeof(F), new object[] { this });
            if (inst == null)
            {
                throw new NullReferenceException($"Could not create instance of {typeof(F)}");
            }
            this.Fsm = inst;
        }

        /// <summary>
        /// The generated FSM class
        /// </summary>
        public F Fsm { get; private set; } = default!;

        /// <summary>
        /// The generated state of the FSM class
        /// </summary>
        public S State
        {
            get { return (S)this.Fsm.GetType().GetProperty("State")!.GetValue(this.Fsm)!; }
            set { this.Fsm.GetType().GetProperty("State")!.SetValue(this.Fsm, value); }
        }

        /// <summary>
        /// Deserialize the already instantiated SMC controller shallowly and call SetOwner on the SMC
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="filter"></param>
        public override void Deserialize(byte[] bytes, Func<byte[], byte[]>? filter = null)
        {
            base.Deserialize(bytes, filter);
            this.SetOwner();
        }

        /// <summary>
        /// Must explicitly be called after deserialization (as the controller itself is not serializable).
        /// </summary>
        private void SetOwner()
        {
            this.Fsm.GetType().GetProperty("Owner")!.SetValue(this.Fsm, this);
        }
    }
}