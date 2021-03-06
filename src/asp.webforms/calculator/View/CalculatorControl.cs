﻿using asp.calculator.Control;
using asplib.View;
using System.Web.UI;

namespace asp.calculator.View

{
    /// <summary>
    /// Base class for all UserControls in the page element hierarchy
    /// </summary>
    public abstract class CalculatorControl : UserControl, ISmcControl<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
    {
        public Calculator Main { get; set; }

        public CalculatorContext Fsm
        {
            get { return this.Main.Fsm; }
        }

        public CalculatorContext.CalculatorState State
        {
            get { return this.Main.State; }
            set { this.Main.State = value; }
        }

        /// <summary>
        /// For dynamic assignment in the .ascx
        /// </summary>
        public string Storage
        {
            get { return this.GetStorage().ToString(); }
            set { this.SetStorage(value); }
        }

        /// <summary>
        /// Local session storage type in the instance, overrides the global config
        /// </summary>
        public Storage? SessionStorage { get; set; }

        /// <summary>
        /// Make the protected ViewState public
        /// </summary>
        public new StateBag ViewState
        {
            get { return base.ViewState; }
        }
    }
}