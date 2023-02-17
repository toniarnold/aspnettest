using asplib.Model;

namespace BlazorApp1
{
    [TestFixture(typeof(EdgeDriver))]
    public class CounterTest<TWebDriver> : StaticOwningComponentTest<TWebDriver, Pages.Counter, Models.CounterModel>
        where TWebDriver : IWebDriver, new()
    {
        [OneTimeSetUp]
        public void SetSessionStorage()
        {
            StorageImplementation.SetStorage(Storage.SessionStorage);  // override aspesttings.json
        }

        [OneTimeTearDown]
        public void UnsetSessionStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        [SetUp]
        public void NavigateToComponent()
        {
            Navigate("/counter");   // the Pages.Counter Component under test
        }

        /// <summary>
        /// Persistence breaks test independence if the session is not cleared after test runs
        /// </summary>
        [TearDown]
        public void ClearSession()
        {
            this.Navigate("/counter?clear=true");
        }

        // duration="6.418394" asserts="203"
        [Test]
        public void CountHtmlAssertion()
        {
            // First the strict model assertion, then the shaky markup assertion:
            Assert.That(Main.CurrentCount, Is.EqualTo(0));
            Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

            // Click multiple times and verify that the HTML contains the current i
            for (int i = 1; i <= 100; i++)
            {
                Click(Cut.incrementButton);
                Assert.That(Main.CurrentCount, Is.EqualTo(i));
                Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>Current count: {i}</p>");
            }
        }

        // duration="3.382291" asserts="203
        [Test]
        public void CountModelOnlyAssertion()
        {
            // First the strict model assertion, then the shaky markup assertion:
            Assert.That(Main.CurrentCount, Is.EqualTo(0));
            Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

            // Click multiple times and verify that the HTML contains the current i
            for (int i = 1; i <= 100; i++)
            {
                Click(Cut.incrementButton);
                Assert.That(Main.CurrentCount, Is.EqualTo(i));
            }
        }

        [Test]
        public void Persistence()
        {
            // Count() runs first and would persist 100 without ClearSession() in [TearDown]
            Assert.That(Main.CurrentCount, Is.EqualTo(0));
            // Omitting the markup test round trip causes the Cut.incrementButton reference to be brittle:
            Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");
            Click(Cut.incrementButton);
            Assert.That(Main.CurrentCount, Is.EqualTo(1));
            Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 1</p>");
            Refresh(expectRenders: 2);  // F5 in the browser
            Assert.That(Main.CurrentCount, Is.EqualTo(1));  // the counter value is persisted
            Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 1</p>");
            Navigate("/counter?clear=true", expectRenders: 2);
            Assert.That(Main.CurrentCount, Is.EqualTo(0));  // the counter value was cleared
            Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");
        }
    }
}