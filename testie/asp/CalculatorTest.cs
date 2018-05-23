using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using iie;

using asplib.View;
using asp.calculator.Control;
using asp.calculator.View;

namespace testie.asp
{
    [TestFixture]
    public class CalculatorTest : IIE
    {
        private IMainControl<Calculator, CalculatorContext, CalculatorContext.CalculatorState> MainControl
        {
            get { return (IMainControl<Calculator, CalculatorContext, CalculatorContext.CalculatorState>)
                            iie.IEExtension.MainControl; }
        }

        private Calculator Main
        {
            get { return this.MainControl.Main; }
        }

        private Stack<string> Stack
        {
            get { return this.MainControl.Main.Stack; }
        }

        private CalculatorContext.CalculatorState State
        {
            get { return this.MainControl.State; }
        }


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.SetUpIE();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.TearDownIE();
        }


        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/asp/default.aspx");
            Assert.That(this.Html(), Does.Contain("RPN calculator"));
            Assert.That(this.Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash)); // "early binding"
        }

        [Test]
        public void InitEnterTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            Assert.That(this.Html(), Does.Contain("Map1.Enter"));
        }

        [Test]
        public void EnterTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "1");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("1"));
            Assert.That(this.Stack.Count, Is.EqualTo(1));
            Assert.That(this.Html(), Does.Contain(" 1\n"));
        }

        [Test]
        public void AddTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "2");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.addButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
            Assert.That(this.Html(), Does.Contain(" 5\n"));
        }

        [Test]
        public void ClrTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "2");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.clrButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ClrAllTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "2");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.clrAllButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void DivTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "12");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.divButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
            Assert.That(this.Html(), Does.Contain(" 4\n"));
        }

        [Test]
        public void MulTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "4");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.mulButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("12"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
            Assert.That(this.Html(), Does.Contain(" 12\n"));
        }

        [Test]
        public void PowTest()
        {

            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "2");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.powButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
            Assert.That(this.Html(), Does.Contain(" 4\n"));
        }

        [Test]
        public void SqrtTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "49");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.sqrtButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
            Assert.That(this.Html(), Does.Contain(" 7\n"));
        }

        [Test]
        public void SubTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "12");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("calculate.subButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("9"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
            Assert.That(this.Html(), Does.Contain(" 9\n"));
        }
    }
}
