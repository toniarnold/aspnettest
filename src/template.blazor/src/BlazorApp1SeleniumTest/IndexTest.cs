namespace BlazorApp1
{
    [TestFixture(typeof(EdgeDriver))]
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