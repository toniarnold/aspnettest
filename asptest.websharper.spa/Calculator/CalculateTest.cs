using iie;
using NUnit.Framework;

namespace asptest.websharper.spa.Calculator
{
    [TestFixture]
    public class CalculateTest : CalculatorTestBase
    {
        [Test]
        public void NavigateIndexTest()
        {
            this.Navigate("/");
            Assert.Multiple(() =>
            {
                Assert.That(this.Html(), Does.Contain("RPN calculator"));
                //Assert.That(this.Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
                //Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash)); // "early binding"
            });
        }
    }
}