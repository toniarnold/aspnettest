using BlazorApp1.Data;
using BlazorApp1.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp1
{
    public class FetchDataTest : BUnitTestContext
    {
        [OneTimeSetUp]
        public void RegisterWeatherForecastService()
        {
            Services.AddSingleton<WeatherForecastService>();
        }

        [Test]
        public void RenderWeatherForecastTest()
        {
            var cut = RenderComponent<FetchData>();
            cut.Find("h1").MarkupMatches("<h1>Weather forecast</h1>");
            var rows = cut.FindAll("table tbody tr");
            Assert.That(rows.Count, Is.EqualTo(5));
            foreach (var row in rows)
            {
                // Assert the data type of the children (the ChildNodes also contain the text, skip it by index)
                Assert.That(DateTime.TryParse(row.ChildNodes[0].TextContent, out var _), Is.True);
                Assert.That(int.TryParse(row.ChildNodes[2].TextContent, out var _), Is.True);
                Assert.That(int.TryParse(row.ChildNodes[4].TextContent, out var _), Is.True);
                Assert.That(row.ChildNodes[6].TextContent, Is.Not.Empty);
            }
        }
    }
}