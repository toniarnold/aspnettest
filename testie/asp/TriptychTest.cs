using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using iie;

using asplib.View;

using testie.asp.calculator;


namespace testie.asp
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public class TriptychTest : TestBase    // calculator TestBase for CircumambulateStorageTypes
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
            this.Navigate("/asp/default.aspx?clear=true&endresponse=true");
            ControlStorageExtension.SessionStorage = Storage.Session;
            this.Navigate("/asp/default.aspx?clear=true&endresponse=true");
            ControlStorageExtension.SessionStorage = null;
        }

        [Test]
        public void NavigateTriptychTest()
        {
            this.Navigate("/asp/triptych.aspx");
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
                Assert.That(this.Html(), Does.Contain("Session Storage: Viewstate"));
                Assert.That(this.Html(), Does.Contain("Session Storage: Session"));
                Assert.That(this.Html(), Does.Contain("Session Storage: Database"));
            });
        }

        [Test]
        public void CircumambulateStorageTypes()
        {
            this.Navigate("/asp/triptych.aspx");
            this.AssertTriptychHtml();
            this.ClickID("StorageLinkViewstate");
            Assert.That(this.Html(), Does.Contain("Session Storage: Viewstate"));
            // Now we also have a reference to a single Main instance:
            Assert.That(this.MainControl.GetStorage(), Is.EqualTo(Storage.Viewstate));
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
