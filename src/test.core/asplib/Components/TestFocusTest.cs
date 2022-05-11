using asplib.Components;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;

namespace test.asplib.Components
{
    internal class FocussedCompoent : ComponentBase
    {
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                TestFocus.Expose(this);
            }
        }

        public void Render()
        {
            this.OnAfterRender(true);
        }
    }

    public class TestFocusTest
    {
        [Test]
        public void SetFocusTest()
        {
            Assert.That(TestFocus.HasFocus(typeof(FocussedCompoent)), Is.False);
            TestFocus.SetFocus(typeof(FocussedCompoent));
            Assert.That(TestFocus.HasFocus(typeof(FocussedCompoent)), Is.True);
        }

        [Test]
        public void InstantiateFocusTest()
        {
            Assert.That(TestFocus.Component, Is.Null);
            TestFocus.SetFocus(typeof(FocussedCompoent));
            var instance = new FocussedCompoent();
            Assert.That(TestFocus.Component, Is.Null); // still, as not yet rendered
            instance.Render();
            Assert.That(TestFocus.Component, Is.Not.Null);  // constructor sets reference
            Assert.That(instance, Is.EqualTo(TestFocus.Component));
            TestFocus.RemoveFocus();
            Assert.That(TestFocus.Component, Is.Null);  // stalled reference removed
        }

        [TearDown]
        public void TearDown()
        {
            TestFocus.RemoveFocus();
        }
    }
}