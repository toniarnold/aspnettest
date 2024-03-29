#pragma warning disable SYSLIB0003
/* <auto-generated>
 * ex: set ro:
 * DO NOT EDIT.
 * generated by smc (http://smc.sourceforge.net/)
 * from file : Accesscode.sm
 */

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;

using apiservice.Controllers;
[Serializable]
[System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]
public sealed class AccesscodeContext :
    statemap.FSMContext,
    ISerializable
{
//---------------------------------------------------------------
// Properties.
//

    public AccesscodeControllerState State
    {
        get
        {
            if (state_ == null)
            {
                throw(
                    new statemap.StateUndefinedException());
            }

            return ((AccesscodeControllerState) state_);
        }
        set
        {
            SetState(value);
        }
    }

    public AccesscodeController Owner
    {
        get
        {
            return (_owner);
        }
        set
        {
            _owner = value;
        }
    }

    public AccesscodeControllerState[] States
    {
        get
        {
            return (_States);
        }
    }

//---------------------------------------------------------------
// Member methods.
//

    public AccesscodeContext(AccesscodeController owner) :
        base (AuthMap.Idle)
    {
        _owner = owner;
    }

    public override void EnterStartState()
    {
        State.Entry(this);
        return;
    }

    public AccesscodeContext(SerializationInfo info, StreamingContext context) :
        base ()
    {
        int stackSize;
        int stateId;

        stackSize = info.GetInt32("stackSize");
        if (stackSize > 0)
        {
            int index;
            String name;

            for (index = (stackSize - 1); index >= 0; --index)
            {
                name = "stackIndex" + index;
                stateId = info.GetInt32(name);
                PushState(_States[stateId]);
            }
        }

        stateId = info.GetInt32("state");
        PushState(_States[stateId]);
    }

    public void Authenticate(string phonenumber)
    {
        transition_ = "Authenticate";
        State.Authenticate(this, phonenumber);
        transition_ = "";
        return;
    }

    public void Verify(string accesscode)
    {
        transition_ = "Verify";
        State.Verify(this, accesscode);
        transition_ = "";
        return;
    }

    public AccesscodeControllerState valueOf(int stateId)
    {
        return(_States[stateId]);
    }

    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
    public void GetObjectData(SerializationInfo info,
                              StreamingContext context)
    {
        int stackSize = 0;

        if (stateStack_ != null)
        {
            stackSize = stateStack_.Count;
        }

        info.AddValue("stackSize", stackSize);

        if (stackSize > 0)
        {
            int index = 0;
            String name;

            foreach (AccesscodeControllerState state in stateStack_)
            {
                name = "stackIndex" + index;
                info.AddValue(name, state.Id);
                ++index;
            }
        }

        info.AddValue("state", state_.Id);

        return;
    }

//---------------------------------------------------------------
// Member data.
//

    [NonSerialized]
    private AccesscodeController _owner;

    // Map state IDs to state objects.
    // Used to deserialize an FSM.
    [NonSerialized]
    private static AccesscodeControllerState[] _States =
    {
        AuthMap.Idle,
        AuthMap.Unverified,
        AuthMap.Verified,
        AuthMap.Denied
    };

//---------------------------------------------------------------
// Inner classes.
//

    [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]
    public abstract class AccesscodeControllerState :
        statemap.State
    {
    //-----------------------------------------------------------
    // Properties.
    //

        public abstract IDictionary<string, int> Transitions
        {
            get;
        }

    //-----------------------------------------------------------
    // Member methods.
    //

        internal AccesscodeControllerState(string name, int id) :
            base (name, id)
        {}

        protected internal virtual void Entry(AccesscodeContext context)
        {}

        protected internal virtual void Exit(AccesscodeContext context)
        {}

        protected internal virtual void Authenticate(AccesscodeContext context, string phonenumber)
        {
            Default(context);
        }

        protected internal virtual void Verify(AccesscodeContext context, string accesscode)
        {
            Default(context);
        }

        protected internal virtual void Default(AccesscodeContext context)
        {
            throw (
                new statemap.TransitionUndefinedException(
                    "State: " +
                    context.State.Name +
                    ", Transition: " +
                    context.GetTransition()));
        }
    }
    [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]

    internal abstract class AuthMap
    {
    //-----------------------------------------------------------
    // Member methods.
    //

    //-----------------------------------------------------------
    // Member data.
    //

        //-------------------------------------------------------
        // Statics.
        //
        [NonSerialized]
        internal static readonly AuthMap_Default.AuthMap_Idle Idle =
            new AuthMap_Default.AuthMap_Idle("AuthMap.Idle", 0);
        [NonSerialized]
        internal static readonly AuthMap_Default.AuthMap_Unverified Unverified =
            new AuthMap_Default.AuthMap_Unverified("AuthMap.Unverified", 1);
        [NonSerialized]
        internal static readonly AuthMap_Default.AuthMap_Verified Verified =
            new AuthMap_Default.AuthMap_Verified("AuthMap.Verified", 2);
        [NonSerialized]
        internal static readonly AuthMap_Default.AuthMap_Denied Denied =
            new AuthMap_Default.AuthMap_Denied("AuthMap.Denied", 3);
        [NonSerialized]
        private static readonly AuthMap_Default Default =
            new AuthMap_Default("AuthMap.Default", -1);

    }

    [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]
    internal class AuthMap_Default :
        AccesscodeControllerState
    {
    //-----------------------------------------------------------
    // Properties.
    //

        public override IDictionary<string, int> Transitions
        {
            get
            {
                return (_transitions);
            }
        }

    //-----------------------------------------------------------
    // Member methods.
    //

        internal AuthMap_Default(string name, int id) :
            base (name, id)
        {}

    //-----------------------------------------------------------
    // Inner classes.
    //

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]
        internal class AuthMap_Idle :
            AuthMap_Default
        {
        //-------------------------------------------------------
        // Properties.
        //

            public override IDictionary<string, int> Transitions
            {
                get
                {
                    return (_transitions);
                }
            }

        //-------------------------------------------------------
        // Member methods.
        //

            internal AuthMap_Idle(string name, int id) :
                base (name, id)
            {}

            protected internal override void Authenticate(AccesscodeContext context, string phonenumber)
            {

                AccesscodeController ctxt = context.Owner;


                context.State.Exit(context);
                context.ClearState();

                try
                {
                    ctxt.SMSAccesscode(phonenumber);
                }
                finally
                {
                    context.State = AuthMap.Unverified;
                    context.State.Entry(context);
                }



                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static AuthMap_Idle()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Authenticate(string phonenumber)", 1);
                _transitions.Add("Verify(string accesscode)", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]
        internal class AuthMap_Unverified :
            AuthMap_Default
        {
        //-------------------------------------------------------
        // Properties.
        //

            public override IDictionary<string, int> Transitions
            {
                get
                {
                    return (_transitions);
                }
            }

        //-------------------------------------------------------
        // Member methods.
        //

            internal AuthMap_Unverified(string name, int id) :
                base (name, id)
            {}

            protected internal override void Verify(AccesscodeContext context, string accesscode)
            {

                AccesscodeController ctxt = context.Owner;

                if ( ctxt.IsValid(accesscode) )
                {

                    context.State.Exit(context);
                    context.ClearState();

                    try
                    {
                        ctxt.SaveAccesscode();
                    }
                    finally
                    {
                        context.State = AuthMap.Verified;
                        context.State.Entry(context);
                    }

                }
                else if ( ctxt.HaveMoreAttempts() )
                {

                    context.State.Exit(context);
                    context.ClearState();

                    try
                    {
                        ctxt.IncrementAttempts();
                    }
                    finally
                    {
                        context.State = AuthMap.Unverified;
                        context.State.Entry(context);
                    }

                }
                else
                {

                    context.State.Exit(context);
                    context.State = AuthMap.Denied;
                    context.State.Entry(context);
                }

                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static AuthMap_Unverified()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Authenticate(string phonenumber)", 0);
                _transitions.Add("Verify(string accesscode)", 1);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]
        internal class AuthMap_Verified :
            AuthMap_Default
        {
        //-------------------------------------------------------
        // Properties.
        //

            public override IDictionary<string, int> Transitions
            {
                get
                {
                    return (_transitions);
                }
            }

        //-------------------------------------------------------
        // Member methods.
        //

            internal AuthMap_Verified(string name, int id) :
                base (name, id)
            {}

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static AuthMap_Verified()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Authenticate(string phonenumber)", 0);
                _transitions.Add("Verify(string accesscode)", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.2.0")]
        internal class AuthMap_Denied :
            AuthMap_Default
        {
        //-------------------------------------------------------
        // Properties.
        //

            public override IDictionary<string, int> Transitions
            {
                get
                {
                    return (_transitions);
                }
            }

        //-------------------------------------------------------
        // Member methods.
        //

            internal AuthMap_Denied(string name, int id) :
                base (name, id)
            {}

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static AuthMap_Denied()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Authenticate(string phonenumber)", 0);
                _transitions.Add("Verify(string accesscode)", 0);
            }
        }

    //-----------------------------------------------------------
    // Member data.
    //

        //-------------------------------------------------------
        // Statics.
        //
        private static IDictionary<string, int> _transitions;

        static AuthMap_Default()
        {
            _transitions = new Dictionary<string, int>();
            _transitions.Add("Authenticate(string phonenumber)", 0);
            _transitions.Add("Verify(string accesscode)", 0);
        }
    }
}


/*
 * Local variables:
 *  buffer-read-only: t
 * End:
 */
