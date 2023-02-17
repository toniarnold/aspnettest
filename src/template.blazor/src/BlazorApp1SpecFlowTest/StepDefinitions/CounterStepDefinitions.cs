using BlazorApp1SpecFlowTest.Drivers;
using iselenium;
using static BlazorApp1SpecFlowTest.Features.CounterFeature;

namespace BlazorApp1SpecFlowTest.StepDefinitions
{
    [Binding, Scope(Feature = "Counter")]
    public sealed class CounterStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        /// <summary>
        /// The layer intermediating between the step definitions and the automation with assertions
        /// </summary>
        private readonly CounterDriver _driver;

        public CounterStepDefinitions(CounterDriver driver)
        {
            _driver = driver;
        }

        [Given(@"^the '(.*)' page is loaded$")]
        public void GivenThePageIsLoaded(string path)
        {
            Driver.Navigate(path);
        }

        [Then(@"^the counter text is '(.*)'$")]
        public void ThenCounterTextIs(string countertext)
        {
            Driver.Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>{countertext}</p>");
        }

        [When("the increment button is clicked (.*) times")]
        public void WhenTheIncrementButtonIsClicked(int times)
        {
            _driver.ClickIncrementButtonAssertCount(times);
        }

        [Then("the counter value is (.*)")]
        public void ThenTheCountercalueIs(int value)
        {
            _driver.AssertCurrentCountIs(value);
        }

        [When("the page is reloaded")]
        public void WhenThePageIsReloaded()
        {
            _driver.ReloadPage();
        }

        [When("the persistence storage is cleared")]
        public void WhenThePersistenceStorageIsCleared()
        {
            _driver.ClearCounterStorage();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _driver.ClearCounterStorage();
        }
    }
}