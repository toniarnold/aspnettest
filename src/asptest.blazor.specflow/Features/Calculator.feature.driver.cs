using asptest.blazor.specflow.Drivers;
using OpenQA.Selenium.Edge;

namespace asptest.blazor.specflow.Features
{
    /// <summary>
    /// Sets the Browser to use and a static instance accessor for the CalculatorStepDefinitions
    /// </summary>
    public partial class CalculatorFeature : CalculatorTestBase<EdgeDriver>
    {
        /// <summary>
        /// The NUnit test class actually running in the server process
        /// </summary>
        public static CalculatorFeature Driver { get; set; } = default!;

        public CalculatorFeature()
        {
            Driver = this;
        }
    }
}