using NUnit.Framework;
using System.Threading;

namespace testbuttontest.blazor
{
    public class TestStatus
    {
        private const int SLEEPTIME = 1000;

        [Test, Order(1)]
        public void Pass()
        {
            Assert.Pass("Passed");
        }

        [Test, Order(2)]
        public void Warn()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Warn("Assert.Warn");
        }

        [Test, Order(3)]
        public void Fail()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Fail("Assert.Fail");
        }

        [Test, Order(4)]
        public void PassInBetween()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Pass("Pass in between");
        }

        [Test, Order(5)]
        public void PauseAtTheEnd()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Pass("End of the tests");
        }
    }
}