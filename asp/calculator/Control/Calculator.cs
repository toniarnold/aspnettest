using System;
using System.Collections.Generic;

using asplib.Control;

namespace asp.calculator.Control
{
    /// <summary>
    /// Context class for Calculator.sm/Calculator_sm.cs
    /// </summary>
    [Serializable]
    public class Calculator : IMain<CalculatorContext, CalculatorContext.CalculatorState>
    {
        protected CalculatorContext _fsm;
        protected Stack<string> _stack;

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
        /// String represenation of the stack for HTML presentation
        /// </summary>
        public string StackHtmlString
        {
           get { return String.Join("<br />", this.Stack); }
        }

        /// <summary>
        /// R is the access point for fhe equivalent of global variables in PHP (volatile between requests)
        /// </summary>
        public string Operand
        {
            get { return this.operand; }
            set { this.operand = value; }
        }
        [NonSerialized]
        private string operand;

        // Context methods
        public void Push(string value)
        {
            this._stack.Push(value);
        }

        public void Enter(string value)
        {
            this.Push(value);
        }

        public void Add()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x + y;
            this.Push(r.ToString());
        }
        public void Sub()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x - y;
            this.Push(r.ToString());
        }
        public void Mul()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x * y;
            this.Push(r.ToString());
        }
        public void Div()
        {
            var y = Double.Parse(this._stack.Pop());
            var x = Double.Parse(this._stack.Pop());
            var r = x / y;
            this.Push(r.ToString());
        }
        public void Pow()
        {
            var x = Double.Parse(this._stack.Pop());
            var r = Math.Pow(x, 2);
            this.Push(r.ToString());
        }
        public void Sqrt()
        {
            var x = Double.Parse(this._stack.Pop());
            var r = Math.Sqrt(x);
            this.Push(r.ToString());
        }
        public void Clr()
        {
            this._stack.Pop();
        }
        public void ClrAll()
        {
            this._stack = new Stack<string>();
        }
    }
}
