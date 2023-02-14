using iselenium;
using static BlazorApp1SpecFlowTest.Features.IndexFeature;

namespace BlazorApp1SpecFlowTest.StepDefinitions
{
    [Binding, Scope(Feature = "Index")]
    public sealed class IndexStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        [Given("the app is initially loaded")]
        public void GivenTheAppIsInitiallyLoaded()
        {
            Driver.Navigate("/");
        }

        [Then(@"^the '(.*)' element matches '(.*)'$")]
        public void ThenTheElementMatches(string element, string match)
        {
            Driver.Find(element).MarkupMatches(match);
        }
    }
}