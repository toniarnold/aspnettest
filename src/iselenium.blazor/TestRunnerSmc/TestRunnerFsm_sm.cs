/*
 * ex: set ro:
 * DO NOT EDIT.
 * generated by smc (http://smc.sourceforge.net/)
 * from file : TestRunnerFsm.sm
 */

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;

using iselenium;
[Serializable]
[System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
public sealed class TestRunnerFsmContext :
    statemap.FSMContext,
    ISerializable
{
//---------------------------------------------------------------
// Properties.
//

    public TestRunnerFsmState State
    {
        get
        {
            if (state_ == null)
            {
                throw(
                    new statemap.StateUndefinedException());
            }

            return ((TestRunnerFsmState) state_);
        }
        set
        {
            SetState(value);
        }
    }

    public TestRunnerFsm Owner
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

    public TestRunnerFsmState[] States
    {
        get
        {
            return (_States);
        }
    }

//---------------------------------------------------------------
// Member methods.
//

    public TestRunnerFsmContext(TestRunnerFsm owner) :
        base (Map1.New)
    {
        _owner = owner;
    }

    public override void EnterStartState()
    {
        State.Entry(this);
        return;
    }

    public TestRunnerFsmContext(SerializationInfo info, StreamingContext context) :
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

    public void Complete()
    {
        transition_ = "Complete";
        State.Complete(this);
        transition_ = "";
        return;
    }

    public void OnTestEvent(TestStatus result)
    {
        transition_ = "OnTestEvent";
        State.OnTestEvent(this, result);
        transition_ = "";
        return;
    }

    public void Report()
    {
        transition_ = "Report";
        State.Report(this);
        transition_ = "";
        return;
    }

    public void RunTests()
    {
        transition_ = "RunTests";
        State.RunTests(this);
        transition_ = "";
        return;
    }

    public TestRunnerFsmState valueOf(int stateId)
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

            foreach (TestRunnerFsmState state in stateStack_)
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
    private TestRunnerFsm _owner;

    // Map state IDs to state objects.
    // Used to deserialize an FSM.
    [NonSerialized]
    private static TestRunnerFsmState[] _States =
    {
        Map1.New,
        Map1.RunningOk,
        Map1.RunningWarning,
        Map1.RunningError,
        Map1.CompletedOk,
        Map1.CompletedWarning,
        Map1.CompletedError,
        Map1.Passed,
        Map1.ResultXml
    };

//---------------------------------------------------------------
// Inner classes.
//

    [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
    public abstract class TestRunnerFsmState :
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

        internal TestRunnerFsmState(string name, int id) :
            base (name, id)
        {}

        protected internal virtual void Entry(TestRunnerFsmContext context)
        {}

        protected internal virtual void Exit(TestRunnerFsmContext context)
        {}

        protected internal virtual void Complete(TestRunnerFsmContext context)
        {
            Default(context);
        }

        protected internal virtual void OnTestEvent(TestRunnerFsmContext context, TestStatus result)
        {
            Default(context);
        }

        protected internal virtual void Report(TestRunnerFsmContext context)
        {
            Default(context);
        }

        protected internal virtual void RunTests(TestRunnerFsmContext context)
        {
            Default(context);
        }

        protected internal virtual void Default(TestRunnerFsmContext context)
        {
            throw (
                new statemap.TransitionUndefinedException(
                    "State: " +
                    context.State.Name +
                    ", Transition: " +
                    context.GetTransition()));
        }
    }
    [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]

    internal abstract class Map1
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
        internal static readonly Map1_Default.Map1_New New =
            new Map1_Default.Map1_New("Map1.New", 0);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_RunningOk RunningOk =
            new Map1_Default.Map1_RunningOk("Map1.RunningOk", 1);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_RunningWarning RunningWarning =
            new Map1_Default.Map1_RunningWarning("Map1.RunningWarning", 2);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_RunningError RunningError =
            new Map1_Default.Map1_RunningError("Map1.RunningError", 3);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_CompletedOk CompletedOk =
            new Map1_Default.Map1_CompletedOk("Map1.CompletedOk", 4);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_CompletedWarning CompletedWarning =
            new Map1_Default.Map1_CompletedWarning("Map1.CompletedWarning", 5);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_CompletedError CompletedError =
            new Map1_Default.Map1_CompletedError("Map1.CompletedError", 6);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_Passed Passed =
            new Map1_Default.Map1_Passed("Map1.Passed", 7);
        [NonSerialized]
        internal static readonly Map1_Default.Map1_ResultXml ResultXml =
            new Map1_Default.Map1_ResultXml("Map1.ResultXml", 8);
        [NonSerialized]
        private static readonly Map1_Default Default =
            new Map1_Default("Map1.Default", -1);

    }

    [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
    internal class Map1_Default :
        TestRunnerFsmState
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

        internal Map1_Default(string name, int id) :
            base (name, id)
        {}

    //-----------------------------------------------------------
    // Inner classes.
    //

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_New :
            Map1_Default
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

            internal Map1_New(string name, int id) :
                base (name, id)
            {}

            protected internal override void RunTests(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.RunningOk;
                context.State.Entry(context);


                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static Map1_New()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 0);
                _transitions.Add("OnTestEvent(TestStatus result)", 0);
                _transitions.Add("Report()", 0);
                _transitions.Add("RunTests()", 1);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_RunningOk :
            Map1_Default
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

            internal Map1_RunningOk(string name, int id) :
                base (name, id)
            {}

            protected internal override void Complete(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.CompletedOk;
                context.State.Entry(context);


                return;
            }

            protected internal override void OnTestEvent(TestRunnerFsmContext context, TestStatus result)
            {
                if (result == TestStatus.Passed)
                {

                    context.State.Exit(context);
                    // No actions.
                    context.State = Map1.RunningOk;
                    context.State.Entry(context);
                }
                else if (result == TestStatus.Warning ||
		 result == TestStatus.Inconclusive ||
		 result == TestStatus.Skipped)
                {

                    context.State.Exit(context);
                    // No actions.
                    context.State = Map1.RunningWarning;
                    context.State.Entry(context);
                }
                else
                {

                    context.State.Exit(context);
                    context.State = Map1.RunningError;
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

            static Map1_RunningOk()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 1);
                _transitions.Add("OnTestEvent(TestStatus result)", 1);
                _transitions.Add("Report()", 0);
                _transitions.Add("RunTests()", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_RunningWarning :
            Map1_Default
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

            internal Map1_RunningWarning(string name, int id) :
                base (name, id)
            {}

            protected internal override void Complete(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.CompletedWarning;
                context.State.Entry(context);


                return;
            }

            protected internal override void OnTestEvent(TestRunnerFsmContext context, TestStatus result)
            {
                if (result == TestStatus.Failed ||
	     result == TestStatus.Unknown)
                {

                    context.State.Exit(context);
                    // No actions.
                    context.State = Map1.RunningError;
                    context.State.Entry(context);
                }
                else
                {

                    context.State.Exit(context);
                    context.State = Map1.RunningWarning;
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

            static Map1_RunningWarning()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 1);
                _transitions.Add("OnTestEvent(TestStatus result)", 1);
                _transitions.Add("Report()", 0);
                _transitions.Add("RunTests()", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_RunningError :
            Map1_Default
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

            internal Map1_RunningError(string name, int id) :
                base (name, id)
            {}

            protected internal override void Complete(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.CompletedError;
                context.State.Entry(context);


                return;
            }

            protected internal override void OnTestEvent(TestRunnerFsmContext context, TestStatus result)
            {

                context.State.Exit(context);
                context.State = Map1.RunningError;
                context.State.Entry(context);


                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static Map1_RunningError()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 1);
                _transitions.Add("OnTestEvent(TestStatus result)", 1);
                _transitions.Add("Report()", 0);
                _transitions.Add("RunTests()", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_CompletedOk :
            Map1_Default
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

            internal Map1_CompletedOk(string name, int id) :
                base (name, id)
            {}

            protected internal override void Report(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.Passed;
                context.State.Entry(context);


                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static Map1_CompletedOk()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 0);
                _transitions.Add("OnTestEvent(TestStatus result)", 0);
                _transitions.Add("Report()", 1);
                _transitions.Add("RunTests()", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_CompletedWarning :
            Map1_Default
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

            internal Map1_CompletedWarning(string name, int id) :
                base (name, id)
            {}

            protected internal override void Report(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.ResultXml;
                context.State.Entry(context);


                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static Map1_CompletedWarning()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 0);
                _transitions.Add("OnTestEvent(TestStatus result)", 0);
                _transitions.Add("Report()", 1);
                _transitions.Add("RunTests()", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_CompletedError :
            Map1_Default
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

            internal Map1_CompletedError(string name, int id) :
                base (name, id)
            {}

            protected internal override void Report(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.ResultXml;
                context.State.Entry(context);


                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static Map1_CompletedError()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 0);
                _transitions.Add("OnTestEvent(TestStatus result)", 0);
                _transitions.Add("Report()", 1);
                _transitions.Add("RunTests()", 0);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_Passed :
            Map1_Default
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

            internal Map1_Passed(string name, int id) :
                base (name, id)
            {}

            protected internal override void RunTests(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.RunningOk;
                context.State.Entry(context);


                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static Map1_Passed()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 0);
                _transitions.Add("OnTestEvent(TestStatus result)", 0);
                _transitions.Add("Report()", 0);
                _transitions.Add("RunTests()", 1);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("smc"," v. 7.1.0")]
        internal class Map1_ResultXml :
            Map1_Default
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

            internal Map1_ResultXml(string name, int id) :
                base (name, id)
            {}

            protected internal override void RunTests(TestRunnerFsmContext context)
            {

                context.State.Exit(context);
                context.State = Map1.RunningOk;
                context.State.Entry(context);


                return;
            }

        //-------------------------------------------------------
        // Member data.
        //

            //---------------------------------------------------
            // Statics.
            //
            new private static IDictionary<string, int> _transitions;

            static Map1_ResultXml()
            {
                _transitions = new Dictionary<string, int>();
                _transitions.Add("Complete()", 0);
                _transitions.Add("OnTestEvent(TestStatus result)", 0);
                _transitions.Add("Report()", 0);
                _transitions.Add("RunTests()", 1);
            }
        }

    //-----------------------------------------------------------
    // Member data.
    //

        //-------------------------------------------------------
        // Statics.
        //
        private static IDictionary<string, int> _transitions;

        static Map1_Default()
        {
            _transitions = new Dictionary<string, int>();
            _transitions.Add("Complete()", 0);
            _transitions.Add("OnTestEvent(TestStatus result)", 0);
            _transitions.Add("Report()", 0);
            _transitions.Add("RunTests()", 0);
        }
    }
}


/*
 * Local variables:
 *  buffer-read-only: t
 * End:
 */