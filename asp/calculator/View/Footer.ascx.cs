using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using asplib.View;

using asp.calculator.Control;


namespace asp.calculator.View
{
    public partial class Footer : CalculatorControl
    {
        protected void enterButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Enter(this.Main.Operand);
        }
    }
}