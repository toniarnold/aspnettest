using asplib.Model.Db;
using static BlazorApp1SpecFlowTest.Features.CounterFeature;

namespace BlazorApp1SpecFlowTest.Drivers
{
    public class CounterDriver : FetchDataDriver
    {
        public void ClickIncrementButtonAssertCount(int times)
        {
            for (int i = 1; i <= times; i++)
            {
                Driver.Click(Driver.Cut.incrementButton);
                Assert.That(Driver.Main.CurrentCount, Is.EqualTo(i));
            }
        }

        public void AssertCurrentCountIs(int value)
        {
            Assert.That(Driver.Main.CurrentCount, Is.EqualTo(value));
        }

        public void ReloadPage()
        {
            Driver.Refresh(expectRenders: 2);
        }

        public void ClearCounterStorage()
        {
            Driver.Navigate("/counter?clear=true");
        }
    }
}