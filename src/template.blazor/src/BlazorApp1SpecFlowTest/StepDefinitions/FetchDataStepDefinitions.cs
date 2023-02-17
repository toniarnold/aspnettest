using BlazorApp1SpecFlowTest.Drivers;
using iselenium;
using Moq;
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

        [Then(@"^the title is '(.*)'$")]
        public void ThenTheTitleIs(string title)
        {
            Driver.Find("h1").MarkupMatches($"<h1 diff:ignoreAttributes>{title}</h1>");
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