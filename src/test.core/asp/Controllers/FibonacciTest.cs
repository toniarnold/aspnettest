extern alias core;

using asplib.Model.Db;
using core::asp.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.Linq;
using CalculatorContext = core::CalculatorContext;

namespace test.asp.Controllers
{
    /// <summary>
    /// Mirror of asptest.asp.calculator without IE, asserting solely on the POCO Control/Model class
    /// </summary>
    [TestFixture]
    [Category("DbContext")]
    public class FibonacciTest
    {
        private IConfigurationRoot config;

        [OneTimeSetUp]
        public void SetUpDbContext()
        {
            this.config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            ASP_DBEntities.SetUpInsulatedDbContext(config["ASP_DBEntities"]);
        }

        [OneTimeTearDown]
        public void TearDownInstulatedDbContext()
        {
            ASP_DBEntities.TearDownInsulatedDbContext();
        }

        [Test]
        public void VerifyFibonacciSums()
        {
            CalculatorController inst;
            using (var db = new ASP_DBEntities())
            {
                var bytes = db.LoadMain(this.config.GetValue<Guid>("asp.Controllers.FibonacciTest"));
                inst = new CalculatorController();
                inst.Deserialize(bytes);
                inst.Fsm.Owner = inst;  // As in ISmcControl.LoadMain<M, F, S>(), see SMC Manual Section 9
            }
            Assert.That(inst.Stack.Count, Is.GreaterThanOrEqualTo(3));  // non-empty sequence
            Assert.That(inst.State, Is.EqualTo(CalculatorContext.Map1.Calculate));

            // Assert the sums backwards on the model objects instead of the GUI
            while (inst.Stack.Count >= 3)
            {
                var initialStackCount = inst.Stack.Count;

                // Get the head of the sequence to check
                var sum = inst.Stack.ElementAt(0);
                var summand1 = inst.Stack.ElementAt(1);
                var summand2 = inst.Stack.ElementAt(2);

                // Check the correctness of the Fibonacci sequence  in the calculator GUI

                // Delete the current sum and recalculate it from the sequence
                inst.Fsm.Clr(inst.Stack);       // this.Click("calculate.clrButton");
                inst.Fsm.Add(inst.Stack);       //this.Click("calculate.addButton");
                Assert.That(inst.Stack.ElementAt(0), Is.EqualTo(sum));

                // Delete the calculated check sum
                inst.Fsm.Clr(inst.Stack);       // this.Click("calculate.clrButton");

                // Put the original summands onto the stack again
                inst.Fsm.Enter("");             // this.Click("footer.enterButton");
                inst.Fsm.Enter(summand2);       // this.Write("enter.operandTextBox", summand2);
                                                // this.Click("footer.enterButton");

                inst.Fsm.Enter("");             // this.Click("footer.enterButton");
                inst.Fsm.Enter(summand1);       // this.Write("enter.operandTextBox", summand1);
                                                // this.Click("footer.enterButton");

                // Check that the loop will terminate by continuing with N-1 elements
                Assert.That(inst.Stack.Count, Is.EqualTo(initialStackCount - 1));
            }
        }
    }
}