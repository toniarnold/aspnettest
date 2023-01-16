namespace BlazorApp1
{
    // Generic Test Fixture to schedule the browsers to run the tests with
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    public class IndexTest<TWebDriver> : StaticComponentTest<TWebDriver, Pages.Index>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void SurveyPromptTitle()
        {
            Navigate("/");
            // The SurveyPrompt Title gets rendered as <strong>@Title</strong>
            Find("strong").MarkupMatches("<strong>How is Blazor working for you?</strong>");
        }
    }
}