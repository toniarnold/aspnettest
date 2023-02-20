using asplib.Services;
using BlazorApp1.Models;
using BlazorApp1.Pages;

namespace BlazorApp1
{
    public class CounterTest : BUnitTestContext
    {
        private const int COUNT_NUMBER = 5000;

        [OneTimeSetUp]
        public void RegisterModelService()
        {
            Services.AddPersistent<CounterModel>();
        }

        // bUnit caches component rendering which interferes with the benchmark
        [Test]
        public void CacheComponent()
        {
            RenderComponent<Counter>();
        }

        [Test]
        public void CountBlackboxTest()     // 2.8 Sek.
        {
            var cut = RenderComponent<Counter>();
            cut.Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

            // Click multiple times and verify that the HTML contains the current i
            for (int i = 1; i <= COUNT_NUMBER; i++)
            {
                cut.Find("#incrementButton").Click();
                cut.Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>Current count: {i}</p>");
            }
        }

        [Test]
        public void CountWhiteboxTest()     // 1.4 Sek.
        {
            var cut = RenderComponent<Counter>();
            Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(0));

            // Click multiple times and verify that the model object contains the current i
            for (int i = 1; i <= COUNT_NUMBER; i++)
            {
                cut.Find("#incrementButton").Click();
                Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(i));
            }
        }
    }
}