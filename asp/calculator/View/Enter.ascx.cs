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
    public partial class Enter : CalculatorControl
    {
        /// <summary>
        /// Always store the opreand into the "global variable" this.R.Operand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.Compare(this.operandTextBox.Text, "except", true) == 0)
            {
                throw new Exception("Deliberate Exception");
            }
            this.Main.Operand = this.operandTextBox.Text;
        }
    }
}