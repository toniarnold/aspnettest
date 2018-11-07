using asplib.Controllers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

[assembly: InternalsVisibleTo("asp.core.Views")]    // for CalculatorContext.Map1 in the Razor engine
[assembly: InternalsVisibleTo("test.core")]
[assembly: InternalsVisibleTo("asptest.core")]

namespace asp.Controllers
{
    /// <summary>
    /// Context class for Calculator.sm/Calculator_sm.cs
    /// </summary>
    public partial class Calculator
    {
        protected Stack<string> stack;

        public Calculator()
        {
            this.Construct();
        }

        public Calculator(IConfigurationRoot config) : base(config) { }

        /// <summary>
        /// Separate constructor method for inheriting in NUnit
        /// </summary>
        protected override void Construct()
        {
            base.Construct();
            this.stack = new Stack<string>();
        }

        public Stack<string> Stack
        {
            get { return this.stack; }
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
            this.stack.Push(value);
        }

        // Action method implementations triggered by the respective acton method in the CalculatorController.cs file
        [NonAction]
        public void Enter(string value)
        {
            this.Push(value);
        }

        [NonAction]
        public void Add()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x + y;
            this.Push(r.ToString());
        }

        [NonAction]
        public void Sub()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x - y;
            this.Push(r.ToString());
        }

        [NonAction]
        public void Mul()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x * y;
            this.Push(r.ToString());
        }

        [NonAction]
        public void Div()
        {
            var y = Double.Parse(this.stack.Pop());
            var x = Double.Parse(this.stack.Pop());
            var r = x / y;
            this.Push(r.ToString());
        }

        [NonAction]
        public void Pow()
        {
            var x = Double.Parse(this.stack.Pop());
            var r = Math.Pow(x, 2);
            this.Push(r.ToString());
        }

        [NonAction]
        public void Sqrt()
        {
            var x = Double.Parse(this.stack.Pop());
            var r = Math.Sqrt(x);
            this.Push(r.ToString());
        }

        [NonAction]
        public void Clr()
        {
            this.stack.Pop();
        }

        [NonAction]
        public void ClrAll()
        {
            this.stack = new Stack<string>();
        }
    }
}