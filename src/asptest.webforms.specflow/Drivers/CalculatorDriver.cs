using static asptest.webforms.specflow.Features.CalculatorFeature;
using NUnit.Framework;
using iselenium;

namespace asptest.webforms.specflow.Drivers
{
    public class CalculatorDriver
    {
        public void EnterTheNumber(int number)
        {
            Driver.Click("footer.enterButton");
            Assert.That(Driver.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            Driver.Write(
                "enter.operandTextBox",
                number.ToString());
            Driver.Click("footer.enterButton");
            Assert.That(Driver.State,
                Is.EqualTo(CalculatorContext.Map1.Calculate));
        }

        public void ClickAdd()
        {
            var before = Driver.Stack.Count;
            Driver.Click(
                "calculate.addButton");
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickSub()
        {
            var before = Driver.Stack.Count;
            Driver.Click("calculate.subButton");
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickMul()
        {
            var before = Driver.Stack.Count;
            Driver.Click("calculate.mulButton");
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickDiv()
        {
            var before = Driver.Stack.Count;
            Driver.Click("calculate.divButton");
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickPow()
        {
            var before = Driver.Stack.Count;
            Driver.Click("calculate.powButton");
            Assert.That(Driver.Stack.Count, Is.EqualTo(before));
        }

        public void ClickSqrt()
        {
            var before = Driver.Stack.Count;
            Driver.Click("calculate.sqrtButton");
            Assert.That(Driver.Stack.Count, Is.EqualTo(before));
        }

        public void AssertResultIs(int result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(Driver.State,
                    Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(Driver.Stack.Peek(),
                    Is.EqualTo(result.ToString()));
                Assert.That(Driver.Html(),
                    Does.Contain(result.ToString()));
            });
        }
    }
}