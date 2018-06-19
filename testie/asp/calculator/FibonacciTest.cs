using iie;
using NUnit.Framework;
using System.Configuration;
using System.Linq;

namespace testie.asp.calculator
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    internal class FibonacciTest : CalculatorTestBase
    {
        [Test]
        public void VerifyFibonacciSums()
        {
            // Load the stored canonical test case
            this.Navigate(string.Format("/asp/default.aspx?session={0}",
                ConfigurationManager.AppSettings["testie.asp.calculator.FibonacciTest"]));
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
                this.Click("calculate.clrButton");
                this.Click("calculate.addButton");
                Assert.That(this.Stack.ElementAt(0), Is.EqualTo(sum));

                // Delete the calculated check sum
                this.Click("calculate.clrButton");

                // Put the original summands onto the stack again
                this.Click("footer.enterButton");
                this.Write("enter.operandTextBox", summand2);
                this.Click("footer.enterButton");

                this.Click("footer.enterButton");
                this.Write("enter.operandTextBox", summand1);
                this.Click("footer.enterButton");

                // Check that the loop will terminate by continuing with N-1 elements
                Assert.That(this.Stack.Count, Is.EqualTo(initialStackCount - 1));
            }
        }
    }
}