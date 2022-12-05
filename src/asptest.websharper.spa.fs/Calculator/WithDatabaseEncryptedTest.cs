using asplib.Model;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace asptest.Calculator
{
    [TestFixture(typeof(EdgeDriver))]
    public class WithDatabaseEncryptedTest<TWebDriver> : WithDatabaseTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        public override void OneTimeSetUpBrowser()
        {
            // Set overriding storage encryption before the browser gets started
            StorageImplementation.EncryptDatabaseStorage = true;
            base.OneTimeSetUpBrowser();
        }

        [OneTimeTearDown]
        public void ResetEncryption()
        {
            StorageImplementation.EncryptDatabaseStorage = null;
        }
    }
}