using asplib.Components;
using asplib.Model;

namespace asp.blazor.CalculatorSmc
{
    /// <summary>
    /// Task class for Calculator.sm/Calculator_sm.cs
    /// </summary>
    [Serializable]
    [Clsid("F39F40BA-4152-4BE0-8907-70F4B135E29D")]
    public class Calculator : IAppClass<CalculatorContext, CalculatorContext.CalculatorState>
    {
        protected CalculatorContext _fsm = default!;
        protected Stack<string> _stack = default!;

        public Calculator()
        {
            this.Construct();
        }

        /// <summary>
        /// Separate constructor method for inheriting in NUnit
        /// </summary>
        protected void Construct()
        {
            this._fsm = new CalculatorContext(this);
            this._stack = new Stack<string>();
        }

        public CalculatorContext Fsm
        {
            get { return this._fsm; }
        }

        public CalculatorContext.CalculatorState State
        {
            get { return this._fsm.State; }
            set { this._fsm.State = value; }
        }

        public Stack<string> Stack
        {
            get { return this._stack; }
        }

        /// <summary>
        /// String representation of the stack for HTML presentation
        /// </summary>
        public string StackHtmlString
        {
            get { return String.Join("<br />", this.Stack); }
        }

        /// <summary>
        /// IMain is the access point for fhe equivalent of global variables in PHP (volatile between requests)
        /// </summary>
        public string Operand
        {
            get { return this.operand; }
            set { this.operand = value; }
        }

        [NonSerialized]
        private string operand;

        //  Context method implementations, called indirectly by the FSM
        //  context class.
        internal void Push(string value)
        {
            this._stack.Push(value);
        }

        internal void Enter(string value)
        {
            this.Push(value);
            this.Operand = String.Empty;    // required in Blazor, in WebForms [NonSerialized]/EnableViewState="false" suffices
        }

        internal void Add()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x + y;
            this.Push(r.ToString());
        }

        internal void Sub()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x - y;
            this.Push(r.ToString());
        }

        internal void Mul()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x * y;
            this.Push(r.ToString());
        }

        internal void Div()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x / y;
            this.Push(r.ToString());
        }

        internal void Pow()
        {
            var x = Double.Parse(this._stack.Pop());
            var r = Math.Pow(x, 2);
            this.Push(r.ToString());
        }

        internal void Sqrt()
        {
            var x = Double.Parse(this._stack.Pop());
            var r = Math.Sqrt(x);
            this.Push(r.ToString());
        }

        internal void Clr()
        {
            this._stack.Pop();
        }

        internal void ClrAll()
        {
            this._stack = new Stack<string>();
        }
    }
}