using iselenium;
using OpenQA.Selenium.Edge;
using BlazorApp1;

namespace BlazorApp1SpecFlowTest.Features
{
    /// <summary>
    /// Partial feature test class specific for the component under test and a browser
    /// </summary>
    public partial class FetchDataFeature : StaticComponentTest<EdgeDriver, BlazorApp1.Pages.FetchData>
    {
        /// <summary>
        /// The NUnit test class actually running in the server process
        /// </summary>
        public static FetchDataFeature Driver { get; set; } = default!;

        public FetchDataFeature()
        {
            Driver = this;
        }
    }
}