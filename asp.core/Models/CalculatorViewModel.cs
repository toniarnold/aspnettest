﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using asp.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace asp.Models
{
    public class CalculatorViewModel
    {
        public string Operand { get; set; }

        [BindNever]
        public CalculatorContext.CalculatorState State { get; set; }

        [BindNever]
        public Stack<string> Stack { get; set; }

        /// <summary>
        /// String representation of the stack for HTML presentation
        /// </summary>
        [BindNever]
        public string StackHtmlString
        {
            get {  return (this.Stack != null) ? String.Join("<br />", this.Stack) : String.Empty; }
        }
    }
}