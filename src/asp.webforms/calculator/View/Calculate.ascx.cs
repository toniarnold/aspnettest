using System;

namespace asp.calculator.View
{
    public partial class Calculate : CalculatorControl
    {
        public string Stack
        {
            get
            {
                return this.Main.StackHtmlString;
            }
        }

        protected void addButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Add(this.Main.Stack);
        }

        protected void subButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Sub(this.Main.Stack);
        }

        protected void mulButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Mul(this.Main.Stack);
        }

        protected void divButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Div(this.Main.Stack);
        }

        protected void powButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Pow(this.Main.Stack);
        }

        protected void sqrtButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Sqrt(this.Main.Stack);
        }

        protected void clrButton_Click(object sender, EventArgs e)
        {
            this.Fsm.Clr(this.Main.Stack);
        }

        protected void clrAllButton_Click(object sender, EventArgs e)
        {
            this.Fsm.ClrAll(this.Main.Stack);
        }
    }
}