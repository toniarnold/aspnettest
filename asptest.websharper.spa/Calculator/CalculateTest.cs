using asp.websharper.spa.Client;
using iie;
using NUnit.Framework;
using static asplib.View.TagHelper;

namespace asptest.websharper.spa.Calculator
{
    [TestFixture]
    public class CalculateTest : CalculatorTestBase
    {
        [Test]
        public void NavigateIndexTest()
        {
            this.Navigate("/");
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash)); // "early binding"
                Assert.That(this.Html(), Does.Contain("RPN calculator SPA"));
                Assert.That(this.Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
            });
        }

        [Test]
        public void InitEnterTest()
        {
            this.Navigate("/");
            this.ClickID(CalculatorDoc.EnterButton);
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
            this.ClickID(Id(CalculatorDoc.EnterButton));
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.WriteID(Id(CalculatorDoc.OperandTextbox), "1");
            this.ClickID(Id(CalculatorDoc.EnterButton));
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("1"));
                Assert.That(this.Stack.Count, Is.EqualTo(1));
                Assert.That(this.Html(), Does.Contain(" 1\n"));
            });
        }
    }
}