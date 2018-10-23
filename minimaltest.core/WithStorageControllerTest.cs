using System;
using System.Collections.Generic;
using System.Text;
using iie;
using NUnit.Framework;
using minimal.Controllers;
using minimal.Models;

namespace minimaltest.core
{
    [TestFixture]
    public class WithStorageControllerTest : IETest
    {
        /// <summary>
        /// There could be a manually generated row in IE's current cookie, thus explicitly delete.
        /// </summary>
        [OneTimeSetUp]
        public void ClearDatabaseStorage()
        {
            this.Navigate(String.Format("/WithStorage?clear=true&storage=database"));
        }

        [Test]
        public void NavigateWithStorageTest()
        {
            this.Navigate("/WithStorage");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }
    }
}
