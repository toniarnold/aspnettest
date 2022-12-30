using BlazorApp1.Models;
using BlazorApp1.Pages;

namespace BlazorApp1
{
    [TestFixture(typeof(EdgeDriver))]
    public class CounterTest<TWebDriver> : StaticOwningComponentTest<TWebDriver, Counter, CounterModel>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void Count()
        {
            Navigate("/counter");
            // First the strict model assertion, then the shaky markup assertion:
            Assert.That(Main.CurrentCount, Is.EqualTo(0));
            Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

            // Click multiple times and verify that the HTML contains the current i
            for (int i = 1; i <= 100; i++)
            {
                Click(Component.incrementButton);
                Assert.That(Main.CurrentCount, Is.EqualTo(i));
                Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>Current count: {i}</p>");
            }
        }

        [Test]
        public void Persistence()
        {
            // Persistence breaks test independence, thus read the old value first:
            Navigate("/counter");
            var initVal = Main.CurrentCount;    // persisted from the Count() test
            Click(Component.incrementButton);
            Assert.That(Main.CurrentCount, Is.EqualTo(initVal + 1));
            Refresh();  // F5 in the browser
            Assert.That(Main.CurrentCount, Is.EqualTo(initVal + 1));  // the counter value is persisted
        }
    }
}