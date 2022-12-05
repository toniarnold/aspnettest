using asp.websharper.spa.Client;
using asplib.Remoting;
using iselenium;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;
using System.Linq;
using static asplib.View.TagHelper;

namespace asptest.Calculator
{
    [TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(ChromeDriver))]
    public class FibonacciTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void VerifyFibonacciSums()
        {
            // Load the stored canonical test case
            this.Navigate(string.Format("/?session={0}",
                    RemotingContext.Configuration.GetValue<Guid>("asptest.Calculator.FibonacciTest")));
            this.AssertPoll(() => this.Stack.Count, () => Is.GreaterThanOrEqualTo(3));  // non-empty sequence
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Calculate));

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
                this.Click(Id(CalculatorDoc.ClrButton), wait: 15);
                this.Click(Id(CalculatorDoc.AddButton));
                this.AssertPoll(() => this.Stack.ElementAt(0), () => Is.EqualTo(sum));

                // Delete the calculated check sum
                this.Click(Id(CalculatorDoc.ClrButton));

                // Put the original summands onto the stack again
                this.Click(Id(CalculatorDoc.EnterButton));
                this.Write(Id(CalculatorDoc.OperandTextbox), summand2);
                this.Click(Id(CalculatorDoc.EnterButton));

                this.Click(Id(CalculatorDoc.EnterButton));
                this.Write(Id(CalculatorDoc.OperandTextbox), summand1);
                this.Click(Id(CalculatorDoc.EnterButton));

                // Check that the loop will terminate by continuing with N-1 elements
                this.AssertPoll(() => this.Stack.Count, () => Is.EqualTo(initialStackCount - 1));
            }
        }
    }
}