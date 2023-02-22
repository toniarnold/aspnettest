using asp.blazor.Components.CalculatorParts;
using static asptest.blazor.specflow.Features.CalculatorFeature;

namespace asptest.blazor.specflow.Drivers
{
    public class CalculatorDriver
    {
        public void EnterTheNumber(int number)
        {
            Driver.Click(Driver.Cut.footer.enterButton);
            Assert.That(Driver.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            Driver.Write(
                Driver.Dynamic<Enter>(Driver.Cut.calculatorPart).operand,
                number.ToString());
            Driver.Click(Driver.Cut.footer.enterButton);
            Assert.That(Driver.State,
                Is.EqualTo(CalculatorContext.Map1.Calculate));
        }

        public void ClickAdd()
        {
            var before = Driver.Stack.Count;
            Driver.Click(
                Driver.Dynamic<Calculate>(Driver.Cut.calculatorPart).addButton);
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickSub()
        {
            var before = Driver.Stack.Count;
            Driver.Click(Driver.Dynamic<Calculate>(Driver.Cut.calculatorPart).subButton);
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickMul()
        {
            var before = Driver.Stack.Count;
            Driver.Click(Driver.Dynamic<Calculate>(Driver.Cut.calculatorPart).mulButton);
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickDiv()
        {
            var before = Driver.Stack.Count;
            Driver.Click(Driver.Dynamic<Calculate>(Driver.Cut.calculatorPart).divButton);
            Assert.That(Driver.Stack.Count, Is.EqualTo(before - 1));
        }

        public void ClickPow()
        {
            var before = Driver.Stack.Count;
            Driver.Click(Driver.Dynamic<Calculate>(Driver.Cut.calculatorPart).powButton);
            Assert.That(Driver.Stack.Count, Is.EqualTo(before));
        }

        public void ClickSqrt()
        {
            var before = Driver.Stack.Count;
            Driver.Click(Driver.Dynamic<Calculate>(Driver.Cut.calculatorPart).sqrtButton);
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