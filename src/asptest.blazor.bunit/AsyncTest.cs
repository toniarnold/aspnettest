using asp.blazor.Pages;
using Bunit;
using iselenium;

namespace asptest.blazor.bunit
{
    public class AsyncTest : BUnitTestContext
    {
        [Test]
        public void NonSynchronized()
        {
            var cut = RenderComponent<Async>();
            Assert.That(cut.Instance.countNumber.Value, Is.EqualTo(Async.Iterations));
            cut.Find(cut.Instance.startButton).Click();
            cut.WaitForState(() => cut.Instance.countNumber.Value == 0,
                                timeout: new TimeSpan(0, 0, 5));    // polls until reached, on my machine 2.1 sec
        }

        [Test]
        public void Synchronized()
        {
            var cut = RenderComponent<Async>();
            Assert.That(cut.Instance.countNumber.Value, Is.EqualTo(Async.Iterations));
            // Will always render once plus additionally half of the iterations:
            cut.Click(cut.Instance.startButton, expectRenders: (Async.Iterations / 2) + 1);
            Assert.That(cut.Instance.countNumber.Value, Is.EqualTo(0));
        }
    }
}