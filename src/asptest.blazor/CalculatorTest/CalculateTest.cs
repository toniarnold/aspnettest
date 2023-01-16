using asp.blazor.Components.CalculatorParts;
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
            Click(Cut.footer.enterButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3.141");
            Click(Cut.footer.enterButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "2");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).addButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "12");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).subButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "4");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).mulButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "12");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).divButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "2");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).powButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "49");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).sqrtButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "2");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).clrButton);
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
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "2");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).clrAllButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Count, Is.EqualTo(0));
            });
        }
    }
}