using iselenium;
using System;

namespace asp.calculator.View
{
    public partial class Enter : CalculatorControl
    {
        /// <summary>
        /// Always store the operand into the "global variable" this.Main.Operand
        /// Locally throw a TestException for malicious input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.Compare(this.operandTextBox.Text, "except", true) == 0)
            {
                throw new TestException("Deliberate Exception");
            }
            this.Main.Operand = this.operandTextBox.Text;
        }
    }
}