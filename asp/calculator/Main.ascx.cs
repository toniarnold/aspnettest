/*
 * Main web control containing the state machine
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using asplib.View;

using asp.calculator.Control;
using asp.calculator.View;

namespace asp.calculator
{
    public partial class Main : CalculatorControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.LoadMain();
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.title.Visible = true;

            if (this.State == CalculatorContext.Map1.Calculate)
            {
                this.calculate.Visible = true;
            }
            else if (this.State == CalculatorContext.Map1.Enter)
            {
                this.enter.Visible = true;
            }
            else if (this.State == CalculatorContext.Map1.ErrorNumeric)
            {
                this.error.Visible = true;
                this.error.Msg = "The input was not numeric.";
            }
            else if (this.State == CalculatorContext.Map1.ErrorTuple)
            {
                this.error.Visible = true;
                this.error.Msg = "Need two values on the stack to compute.";
            }
            else if (this.State == CalculatorContext.Map1.ErrorEmpty)
            {
                this.error.Visible = true;
                this.error.Msg = "Need a value on the stack to compute.";
            }
            else if (this.State == CalculatorContext.Map1.Splash)
            {
                this.splash.Visible = true;
            }
            else
            {
                throw new NotImplementedException(String.Format("this.State {0}", this.State));
            }

            this.footer.Visible = true;

            this.SaveMain();
            base.OnPreRender(e);
        }
    }
}