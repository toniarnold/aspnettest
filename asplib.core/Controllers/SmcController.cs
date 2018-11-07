using Microsoft.Extensions.Configuration;
using System;

namespace asplib.Controllers
{
    // SMC Manual Section 9
    // SMC considers the code it generates to be subservient to the application code. For this reason the SMC code does not serialize
    // its references to the FSM context owner or property listeners.The application code after deserializing
    // the FSM must call the Owner property setter to re - establish the application/ FSM link.
    // If the application listens for FSM state transitions, then event handlers must also be put back in place.

    /// <summary>
    /// Specialization of SerializableController with State
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public class SmcController<F, S> : SerializableController
        where F : statemap.FSMContext
        where S : statemap.State
    {
        public SmcController()
        {
        }   // NUnit

        public SmcController(IConfigurationRoot configuration) : base(configuration)
        {
            this.Construct();
        }

        protected virtual void Construct()
        {
            // Create manually due to constructor argument:
            this.Fsm = (F)Activator.CreateInstance(typeof(F), new object[] { this });
        }

        /// <summary>
        /// The generated FSM class
        /// </summary>
        public F Fsm { get; private set; }

        /// <summary>
        /// The state of the FSM class
        /// </summary>
        public S State
        {
            get { return (S)this.Fsm.GetType().GetProperty("State").GetValue(this.Fsm); }
            set { this.Fsm.GetType().GetProperty("State").SetValue(this.Fsm, value); }
        }

        /// <summary>
        /// Deserialize the already instantiated SMC controller shallowly and call SetOwner on the SMC
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="filter"></param>
        public override void Deserialize(byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            base.Deserialize(bytes, filter);
            this.SetOwner();
        }

        /// <summary>
        /// Must explicitly be called after deserialization (as the controller itself is not serializable).
        /// </summary>
        private void SetOwner()
        {
            this.Fsm.GetType().GetProperty("Owner").SetValue(this.Fsm, this);
        }
    }
}