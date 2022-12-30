namespace BlazorApp1
{
    public class IndexTest : BUnitTestContext
    {
        [Test]
        public void SurveyPromptTitle()
        {
            var cut = RenderComponent<Pages.Index>();
            // The SurveyPrompt Title gets rendered as <strong>@Title</strong>
            cut.Find("strong").MarkupMatches("<strong>How is Blazor working for you?</strong>");
        }
    }
}