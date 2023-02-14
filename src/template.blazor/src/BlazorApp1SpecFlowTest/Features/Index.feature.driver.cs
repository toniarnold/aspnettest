using iselenium;
using OpenQA.Selenium.Edge;

namespace BlazorApp1SpecFlowTest.Features
{
    /// <summary>
    /// Partial feature test class specific for the component under test and a browser
    /// </summary>
    public partial class IndexFeature : StaticComponentTest<EdgeDriver, BlazorApp1.Pages.Index>
    {
        /// <summary>
        /// The NUnit test class actually running in the server process
        /// </summary>
        public static IndexFeature Driver { get; set; } = default!;

        public IndexFeature()
        {
            Driver = this;
        }
    }
}