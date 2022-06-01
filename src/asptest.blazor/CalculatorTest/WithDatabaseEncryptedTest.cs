using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asplib.Model;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

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