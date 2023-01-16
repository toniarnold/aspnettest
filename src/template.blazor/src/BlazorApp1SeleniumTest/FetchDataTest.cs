namespace BlazorApp1
{
    [TestFixture(typeof(EdgeDriver))]
    public class FetchDataTest<TWebDriver> : StaticComponentTest<TWebDriver, Pages.FetchData>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void RenderWeatherForecastTest()
        {
            Navigate("/fetchdata");
            // ignoreAttributes due to the inserted tabindex not present in bUnit
            Find("h1").MarkupMatches("<h1 diff:ignoreAttributes>Weather forecast</h1>");
            var rows = FindAll("table tbody tr");
            Assert.That(rows.Count, Is.EqualTo(5));
            foreach (var row in rows)
            {
                // Assert the data type of the children
                var rowElements = row.FindElements(By.CssSelector("td"));
                Assert.That(DateTime.TryParse(rowElements[0].Text, out var _), Is.True);
                Assert.That(int.TryParse(rowElements[1].Text, out var _), Is.True);
                Assert.That(int.TryParse(rowElements[2].Text, out var _), Is.True);
                Assert.That(rowElements[3].Text, Is.Not.Empty);
            }
        }
    }
}