using asptest.webforms.specflow.Drivers;
using OpenQA.Selenium.Edge;

namespace asptest.webforms.specflow.Features
{
    /// <summary>
    /// Sets the Browser to use and a static instance accessor for the CalculatorStepDefinitions
    /// </summary>
    public partial class CalculatorFeature : CalculatorTestBase
    {
        /// <summary>
        /// The NUnit test class actually running in the server process
        /// </summary>
        public static CalculatorFeature Driver { get; set; }

        public CalculatorFeature()
        {
            Driver = this;
        }
    }
}