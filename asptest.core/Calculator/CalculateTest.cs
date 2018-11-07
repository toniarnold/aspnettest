using iie;
using NUnit.Framework;
using asp.Controllers;

namespace asptest.Calculator
{
    [TestFixture]
    public class CalculateTest : CalculatorTestBase
    {
        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/");
            Assert.Multiple(() =>
            {
                Assert.That(this.Html(), Does.Contain("RPN calculator"));
                Assert.That(this.Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash)); // "early binding"
            });
        }

        [Test]
        public void InitEnterTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
                Assert.That(this.Html(), Does.Contain("Map1.Enter"));
            });
        }

        [Test]
        public void EnterTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "1");
            this.Click("EnterButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("1"));
                Assert.That(this.Stack.Count, Is.EqualTo(1));
                Assert.That(this.Html(), Does.Contain(" 1\n"));
            });
        }

        [Test]
        public void AddTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "2");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("AddButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain(" 5\n"));
            });
        }

        [Test]
        public void ClrTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "2");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("ClrButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
            });
        }

        [Test]
        public void ClrAllTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "2");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("ClrAllButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void DivTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "12");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("DivButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain(" 4\n"));
            });
        }

        [Test]
        public void MulTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "4");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("MulButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("12"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain(" 12\n"));
            });
        }

        [Test]
        public void PowTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "2");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("PowButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
                Assert.That(this.Stack.Count, Is.EqualTo(before));
                Assert.That(this.Html(), Does.Contain(" 4\n"));
            });
        }

        [Test]
        public void SqrtTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "49");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("SqrtButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
                Assert.That(this.Stack.Count, Is.EqualTo(before));
                Assert.That(this.Html(), Does.Contain(" 7\n"));
            });
        }

        [Test]
        public void SubTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "12");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click("SubButton");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("9"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain(" 9\n"));
            });
        }
    }
}
