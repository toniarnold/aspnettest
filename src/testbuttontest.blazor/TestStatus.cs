using NUnit.Framework;
using System;
using System.Threading;

namespace testbuttontest.blazor
{
    public class TestStatus
    {
        private const int SLEEPTIME = 1000;

        [Test, Order(10)]
        public void Pass()
        {
            Assert.Pass("Passed");
        }

        [Test, Order(20)]
        public void Warn()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Warn("Assert.Warn");
        }

        [Test, Order(30)]
        public void PassInBetween1()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Pass("Pass in between");
        }

        [Test, Order(40)]
        public void Fail()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Fail("Assert.Fail");
        }

        [Test, Order(50)]
        public void PassInBetween2()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Pass("Pass in between");
        }

        [Test, Order(60)]
        public void Exception()
        {
            Thread.Sleep(SLEEPTIME);
            throw new Exception("Test-Exception");
        }

        [Test, Order(70)]
        public void PauseAtTheEnd()
        {
            Thread.Sleep(SLEEPTIME);
            Assert.Pass("End of the tests");
        }
    }
}