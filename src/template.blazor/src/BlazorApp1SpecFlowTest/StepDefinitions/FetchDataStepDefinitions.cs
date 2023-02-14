using BlazorApp1SpecFlowTest.Drivers;
using iselenium;
using static BlazorApp1SpecFlowTest.Features.FetchDataFeature;

namespace BlazorApp1SpecFlowTest.StepDefinitions
{
    [Binding, Scope(Feature = "FetchData")]
    public sealed class FetchDataStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        /// <summary>
        /// The layer intermediating between the step definitions and the automation with assertions
        /// </summary>
        private readonly CounterDriver _driver;

        public FetchDataStepDefinitions(CounterDriver driver)
        {
            _driver = driver;
        }

        [Given(@"^the '(.*)' page is loaded$")]
        public void GivenThePageIsLoaded(string path)
        {
            Driver.Navigate(path);
        }

        [Then(@"^the '(.*)' element matches '(.*)'$")]
        public void ThenTheElementMatches(string element, string match)
        {
            Driver.Find(element).MarkupMatches(match);
        }

        [Then("the table has (.*) rows")]
        public void ThenTheTableHasRows(int rows)
        {
            _driver.RowsCountIs(rows);
        }

        [Then("all table cells contain the appropriate data type")]
        public void ThenAllTableCellContainTheAppropriateDataType()
        {
            _driver.AssertCellsDataType();
        }
    }
}