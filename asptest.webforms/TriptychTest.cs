using asplib.View;
using asptest.calculator;
using iselenium;
using NUnit.Framework;

namespace asptest
{
    [TestFixture]
    public class TriptychTest : CalculatorTestBase    // calculator TestBase for CircumambulateStorageTypes
    {
        [SetUp]
        public void UnsetStorage()
        {
            ControlStorageExtension.SessionStorage = null;
        }

        [TearDown]
        public void ClearStorage()
        {
            ControlStorageExtension.SessionStorage = Storage.Database;
            this.Navigate("/default.aspx?clear=true&endresponse=true");
            ControlStorageExtension.SessionStorage = Storage.Session;
            this.Navigate("/default.aspx?clear=true&endresponse=true");
            ControlStorageExtension.SessionStorage = null;
        }

        [Test]
        public void NavigateTriptychTest()
        {
            this.Navigate("/triptych.aspx");
            this.AssertTriptychHtml();
        }

        /// <summary>
        /// We can't use the member navigation as there are 3 competing Calculator instances,
        /// thus assert the presence of the three calculators superficially by text.
        /// </summary>
        private void AssertTriptychHtml()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.Html(), Does.Contain("Session Storage: ViewState"));
                Assert.That(this.Html(), Does.Contain("Session Storage: Session"));
                Assert.That(this.Html(), Does.Contain("Session Storage: Database"));
            });
        }

        [Test]
        public void CircumambulateStorageTypes()
        {
            this.Navigate("/triptych.aspx");
            this.AssertTriptychHtml();
            this.ClickID("StorageLinkViewState");
            Assert.That(this.Html(), Does.Contain("Session Storage: ViewState"));
            // Now we also have a reference to a single Main instance:
            Assert.That(this.MainControl.GetStorage(), Is.EqualTo(Storage.ViewState));
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash));

            this.ClickID("TriptychLink");
            this.AssertTriptychHtml();
            this.ClickID("StorageLinkSession");
            Assert.That(this.Html(), Does.Contain("Session Storage: Session"));
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash));
            Assert.That(this.MainControl.GetStorage(), Is.EqualTo(Storage.Session));
            // Assert that the overridden storage is locally persisted (in the ViewState)
            this.Click("footer.enterButton");
            Assert.That(this.MainControl.GetStorage(), Is.EqualTo(Storage.Session));

            this.ClickID("TriptychLink");
            this.AssertTriptychHtml();
            this.ClickID("StorageLinkDatabase");
            Assert.That(this.Html(), Does.Contain("Session Storage: Database"));
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash));
            Assert.That(this.MainControl.GetStorage(), Is.EqualTo(Storage.Database));
            this.Click("footer.enterButton");
            Assert.That(this.MainControl.GetStorage(), Is.EqualTo(Storage.Database));
        }
    }
}