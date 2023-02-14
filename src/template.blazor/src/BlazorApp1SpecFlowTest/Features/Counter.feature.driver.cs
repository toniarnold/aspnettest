using iselenium;
using OpenQA.Selenium.Edge;
using BlazorApp1;

namespace BlazorApp1SpecFlowTest.Features
{
    /// <summary>
    /// Partial feature test class specific for the component under test and a browser
    /// </summary>
    public partial class CounterFeature : StaticOwningComponentTest<EdgeDriver, BlazorApp1.Pages.Counter, BlazorApp1.Models.CounterModel>
    {
        /// <summary>
        /// The NUnit test class actually running in the server process
        /// </summary>
        public static CounterFeature Driver { get; set; } = default!;

        public CounterFeature()
        {
            Driver = this;
        }
    }
}