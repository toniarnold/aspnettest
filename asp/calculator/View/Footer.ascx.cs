using System;

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