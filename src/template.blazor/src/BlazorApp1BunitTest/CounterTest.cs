using asplib.Services;
using BlazorApp1.Models;
using BlazorApp1.Pages;

namespace BlazorApp1
{
    public class CounterTest : BUnitTestContext
    {
        [OneTimeSetUp]
        public void RegisterModelService()
        {
            Services.AddPersistent<CounterModel>();
        }

        [Test]
        public void CountHtmlAssertion()
        {
            var cut = RenderComponent<Counter>();
            // First the strict model assertion, then the shaky markup assertion:
            Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(0));
            cut.Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

            // Click multiple times and verify that the HTML contains the current i
            for (int i = 1; i <= 100; i++)
            {
                cut.Find("#incrementButton").Click();
                Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(i));
                cut.Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>Current count: {i}</p>");
            }
        }

        [Test]
        public void CountModelOnlyAssertion()
        {
            var cut = RenderComponent<Counter>();
            // First the strict model assertion, then the shaky markup assertion:
            Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(0));
            cut.Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

            // Click multiple times and verify that the HTML contains the current i
            for (int i = 1; i <= 100; i++)
            {
                cut.Find("#incrementButton").Click();
                Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(i));
                cut.Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>Current count: {i}</p>");
            }
        }
    }
}