using asp.blazor.Components.CalculatorParts;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace asptest.CalculatorTest
{
    //[TestFixture(typeof(ChromeDriver))]
    //[TestFixture(typeof(FirefoxDriver))]
    //[TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(EdgeDriver))]
    public class CalculateTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void NavigateDefaultTest()
        {
            Navigate("/");
            Assert.Multiple(() =>
            {
                Assert.That(Html(), Does.Contain("RPN calculator"));
                Assert.That(Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
            });
        }

        [Test]
        public void InitEnterTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
                Assert.That(this.Html(), Does.Contain("Map1.Enter"));
            });
        }

        [Test]
        public void EnterTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3.141");
            Click(Component.footer.enterButton);
            Assert.Multiple(() =>
            {
                Assert.That(State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(Stack.Peek(), Is.EqualTo("3.141"));
                Assert.That(Stack.Count, Is.EqualTo(1));
                Assert.That(Html(), Does.Contain("3.141"));
            });
        }

        [Test]
        public void AddTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "2");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).addButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain("5"));
            });
        }

        [Test]
        public void SubTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "12");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).subButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("9"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain("9"));
            });
        }

        [Test]
        public void MulTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "4");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).mulButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("12"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain("12"));
            });
        }

        [Test]
        public void DivTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "12");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).divButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain("4"));
            });
        }

        [Test]
        public void PowTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "2");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).powButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
                Assert.That(this.Stack.Count, Is.EqualTo(before));
                Assert.That(this.Html(), Does.Contain("4"));
            });
        }

        [Test]
        public void SqrtTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "49");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).sqrtButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
                Assert.That(this.Stack.Count, Is.EqualTo(before));
                Assert.That(this.Html(), Does.Contain("7"));
            });
        }

        [Test]
        public void ClrTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "2");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).clrButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
            });
        }

        [Test]
        public void ClrAllTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "2");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Component.calculatorPart).clrAllButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Count, Is.EqualTo(0));
            });
        }
    }
}