using asp.blazor.Pages;
using asplib.Model;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace asptest
{
    [TestFixture(typeof(EdgeDriver))]
    public class TriptychTest<TWebDriver> : StaticComponentTest<TWebDriver, Triptych>
        where TWebDriver : IWebDriver, new()
    {
        [SetUp]
        public void UnsetStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        [Test]
        public void NavigateTriptychTest()
        {
            Navigate("/triptych");
            AssertTriptychHtml();
            AssertCalculatorInstances();
        }

        [Test]
        public void CircumambulateStorageTypes()
        {
            // In focus is the triptych -> no Cut instance from the main page
            Navigate("/", expectRenders: 0);
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));  // per default disabled for tests
            this.Click(By.Id, "triptychLink");
            this.AssertTriptychHtml();
            this.Click(Cut.calculatorLocalStorage.storageLink, expectRequest: true, expectRenders: 0); // not in focus
            Assert.That(StorageImplementation.SessionStorage, Is.EqualTo(Storage.LocalStorage));
            Assert.That(this.Html(), Does.Contain("Session Storage: LocalStorage"));

            this.Click(By.Id, "triptychLink");
            this.AssertTriptychHtml();
            this.Click(Cut.calculatorSessionStorage.storageLink, expectRequest: true, expectRenders: 0); // not in focus
            Assert.That(StorageImplementation.SessionStorage, Is.EqualTo(Storage.SessionStorage));
            Assert.That(this.Html(), Does.Contain("Session Storage: SessionStorage"));

            // Dynamically switching to Database storage is broken
            //this.Click(By.Id, "triptychLink");
            //this.AssertTriptychHtml();
            //this.Click(Cut.calculatorDatabase.storageLink, expectRequest: true, expectRenders: 0); // not in focus
            //Assert.That(StorageImplementation.SessionStorage, Is.EqualTo(Storage.Database));
            //Assert.That(this.Html(), Does.Contain("Session Storage: Database"));
        }

        /// <summary>
        /// From ASP.NET WebForms: assert the presence of the three calculators
        /// superficially by text, as it had no type specific TestFoxus.
        /// </summary>
        private void AssertTriptychHtml()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.Html(), Does.Contain("Session Storage: SessionStorage"));
                Assert.That(this.Html(), Does.Contain("Session Storage: LocalStorage"));
                Assert.That(this.Html(), Does.Contain("Session Storage: Database"));
            });
        }

        /// <summary>
        /// For Blazor: We can navigate to the individual sub-components.
        /// </summary>
        private void AssertCalculatorInstances()
        {
            // Assert the instances of the Calculator AppClass
            Assert.Multiple(() =>
            {
                Assert.That(Cut?.calculatorSessionStorage?.Main, Is.Not.Null);
                Assert.That(Cut?.calculatorLocalStorage?.Main, Is.Not.Null);
                Assert.That(Cut?.calculatorDatabase?.Main, Is.Not.Null);
            });

            // Assert that all the calculators are in the start state (no
            // stalled persistent instances)
            Assert.Multiple(() =>
            {
                Assert.That(Cut?.calculatorSessionStorage?.Main.State, Is.EqualTo(CalculatorContext.Map1.Splash));
                Assert.That(Cut?.calculatorLocalStorage?.Main.State, Is.EqualTo(CalculatorContext.Map1.Splash));
                Assert.That(Cut?.calculatorDatabase?.Main.State, Is.EqualTo(CalculatorContext.Map1.Splash));
            });

            // Assert distinct local storage types set by Blazor [Parameter]
            Assert.Multiple(() =>
            {
                Assert.That(Cut?.calculatorSessionStorage?.Storage, Is.EqualTo("SessionStorage"));
                Assert.That(Cut?.calculatorLocalStorage?.Storage, Is.EqualTo("LocalStorage"));
                Assert.That(Cut?.calculatorDatabase?.Storage, Is.EqualTo("Database"));
            });
        }
    }
}