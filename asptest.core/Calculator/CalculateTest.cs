using iie;
using NUnit.Framework;

namespace asptest.Calculator
{
    [TestFixture]
    public class CalculateTest : IETest
    {
        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/");
            Assert.Multiple(() =>
            {
                Assert.That(this.Html(), Does.Contain("Calculator"));
            });
        }
    }
}
