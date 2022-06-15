using asplib.Model;
using NUnit.Framework;
using OpenQA.Selenium;

namespace asptest.CalculatorTest
{
    public class WithDatabaseEncryptedTest<TWebDriver> : WithDatabaseTest<TWebDriver>
      where TWebDriver : IWebDriver, new()
    {
        [OneTimeSetUp]
        public void EnableEncryption()
        {
            StorageImplementation.EncryptDatabaseStorage = true;
        }

        [OneTimeTearDown]
        public void ResetEncryption()
        {
            StorageImplementation.EncryptDatabaseStorage = null;
        }
    }
}