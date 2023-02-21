using asptest.blazor.specflow.Drivers;
using static asptest.blazor.specflow.Features.CalculatorFeature;

namespace asptest.blazor.specflow.StepDefinitions
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        /// <summary>
        /// The layer intermediating between the step definitions and the automation with assertions
        /// </summary>
        private readonly CalculatorDriver _driver;

        public CalculatorStepDefinitions(CalculatorDriver driver)
        {
            _driver = driver;
        }

        [BeforeScenario]
        public void NavigateToCalculatorComponent()
        {
            Driver.Navigate("/");
        }

        [Given("the first number is (.*)")]
        public void GivenTheFirstNumberIs(int number)
        {
            _driver.EnterTheNumber(number);
        }

        [Given("the second number is (.*)")]
        public void GivenTheSecondNumberIs(int number)
        {
            _driver.EnterTheNumber(number);
        }

        [When("the add button is clicked")]
        public void WhenTheTwoNumbersAreAdded()
        {
            _driver.ClickAdd();
        }

        [When("the subtract button is clicked")]
        public void WhenTheTwoNumbersAreSubtracted()
        {
            _driver.ClickSub();
        }

        [When("the multiply button is clicked")]
        public void WhenTheTwoNumbersAreMultiplied()
        {
            _driver.ClickMul();
        }

        [When("the divide button is clicked")]
        public void WhenTheTwoNumbersAreDivided()
        {
            _driver.ClickDiv();
        }

        [When("the square button is clicked")]
        public void WhenTheNumberIsSquared()
        {
            _driver.ClickPow();
        }

        [When("the square root button is clicked")]
        public void WhenTheNumberIsSquareRooted()
        {
            _driver.ClickSqrt();
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int result)
        {
            _driver.AssertResultIs(result);
        }
    }
}