using asp.blazor.Components.CalculatorParts;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;
using System.Linq;

namespace asptest.CalculatorTest
{
    [TestFixture(typeof(EdgeDriver))]
    public class FibonacciTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        private IConfiguration _config = default!;

        [OneTimeSetUp]
        public void SetUpConfig()
        {
            _config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .Build();
        }

        [Test]
        public void VerifyFibonacciSums()
        {
            // Load the stored canonical test case
            this.Navigate($"/?session={_config.GetValue<Guid>("asptest.CalculatorTest.FibonacciTest")}");
            Assert.That(this.Stack.Count, Is.GreaterThanOrEqualTo(3));  // non-empty sequence
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));

            // Assert the sums backwards in the GUI
            // Note that the test uses string comparison, there is no arithmetic involved
            // in the assertions - the test gives no hint that it is about math!
            while (this.Stack.Count >= 3)
            {
                var initialStackCount = this.Stack.Count;

                // Get the head of the sequence to check
                var sum = this.Stack.ElementAt(0);
                var summand1 = this.Stack.ElementAt(1);
                var summand2 = this.Stack.ElementAt(2);

                // Check the correctness of the Fibonacci sequence  in the calculator GUI

                // Delete the current sum and recalculate it from the sequence
                this.Click(Dynamic<Calculate>(Component.calculatorPart).clrButton);
                this.Click(Dynamic<Calculate>(Component.calculatorPart).addButton);
                Assert.That(this.Stack.ElementAt(0), Is.EqualTo(sum));

                // Delete the calculated check sum
                this.Click(Dynamic<Calculate>(Component.calculatorPart).clrButton);

                // Put the original summands onto the stack again
                this.Click(Component.footer.enterButton);
                this.Write(Dynamic<Enter>(Component.calculatorPart).operand, summand2);
                this.Click(Component.footer.enterButton);

                this.Click(Component.footer.enterButton);
                this.Write(Dynamic<Enter>(Component.calculatorPart).operand, summand1);
                this.Click(Component.footer.enterButton);

                // Check that the loop will terminate by continuing with N-1 elements
                Assert.That(this.Stack.Count, Is.EqualTo(initialStackCount - 1));
            }
        }
    }
}