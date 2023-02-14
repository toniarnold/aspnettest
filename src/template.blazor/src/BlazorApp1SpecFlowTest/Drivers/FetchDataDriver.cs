using OpenQA.Selenium;
using static BlazorApp1SpecFlowTest.Features.FetchDataFeature;

namespace BlazorApp1SpecFlowTest.Drivers
{
    public class FetchDataDriver
    {
        public void RowsCountIs(int expected)
        {
            var rows = Driver.FindAll("table tbody tr");
            Assert.That(rows.Count, Is.EqualTo(expected));
        }

        public void AssertCellsDataType()
        {
            var rows = Driver.FindAll("table tbody tr");
            foreach (var row in rows)
            {
                // Assert the data type of the children
                var rowElements = row.FindElements(By.CssSelector("td"));
                // SpecFlow sets the culture to the default en-US, ignoring the actual culture of the
                // web application under test, thus omit DateTime parsing for now:
                //Assert.That(DateTime.TryParse(rowElements[0].Text, out var _), Is.True, rowElements[0].Text);
                Assert.That(int.TryParse(rowElements[1].Text, out var _), Is.True, rowElements[1].Text);
                Assert.That(int.TryParse(rowElements[2].Text, out var _), Is.True, rowElements[2].Text);
                Assert.That(rowElements[3].Text, Is.Not.Empty, rowElements[3].Text);
            }
        }
    }
}