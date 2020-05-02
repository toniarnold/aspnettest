using asplib.Model;
using System;
using System.Collections.Generic;

namespace asp.websharper.spa.Model
{
    [Serializable]
    public class SerializableStack : Stack<string> { }

    /// <summary>
    /// Task class implemeentation for the persisted Calculator.sm/Calculator_sm.cs
    /// </summary>
    [Serializable]
    [Clsid("C718C2DA-0C9E-4A52-951C-C0739DE28B7E")]
    public class Calculator : ISmcTask<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
    {
        #region IStored<M>

        [NonSerialized] // not possible on auto property
        private ViewModel<Calculator> viewModel;

        public ViewModel<Calculator> ViewModel
        {
            get { return this.viewModel; }
            set { this.viewModel = value; }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Save();
                }
                disposedValue = true;
            }
        }

        #endregion IStored<M>

        public Calculator()
        {
            this.Construct();
        }

        /// <summary>
        /// Separate constructor method for inheriting in NUnit
        /// </summary>
        protected void Construct()
        {
            this.Fsm = new CalculatorContext(this);
            this.stack = new SerializableStack();
        }

        /// <summary>
        /// Persisted model data: the stack
        /// </summary>
        protected SerializableStack stack;

        /// <summary>
        /// Expose the FSM stack to the transition callers in Remoting for
        /// passing it in as a transition argument.
        /// </summary>
        /// <value>
        /// The FSM's stack.
        /// </value>
        public Stack<string> Stack
        {
            get { return this.stack; }
        }

        /// <summary>
        /// Expose the FSM context class to call transition on from Remoting
        /// </summary>
        /// <value>
        /// The FSM context class.
        /// </value>
        public CalculatorContext Fsm { get; private set; }

        /// <summary>
        /// Expose the State to Remoting.
        /// </summary>
        /// <value>
        /// The FSM state.
        /// </value>
        public CalculatorContext.CalculatorState State
        {
            get { return this.GetState<Calculator, CalculatorContext, CalculatorContext.CalculatorState>(); }
        }

        //  Context method implementations, called indirectly by the FSM
        //  context class.
        internal void Push(string value)
        {
            this.stack.Push(value);
        }

        internal void Enter(string value)
        {
            this.Push(value);
        }

        internal void Add()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x + y;
            this.Push(r.ToString());
        }

        internal void Sub()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x - y;
            this.Push(r.ToString());
        }

        internal void Mul()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x * y;
            this.Push(r.ToString());
        }

        internal void Div()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x / y;
            this.Push(r.ToString());
        }

        internal void Pow()
        {
            var x = Double.Parse(this.stack.Pop());
            var r = Math.Pow(x, 2);
            this.Push(r.ToString());
        }

        internal void Sqrt()
        {
            var x = Double.Parse(this.stack.Pop());
            var r = Math.Sqrt(x);
            this.Push(r.ToString());
        }

        internal void Clr()
        {
            this.stack.Pop();
        }

        internal void ClrAll()
        {
            this.stack = new SerializableStack();
        }
    }
}