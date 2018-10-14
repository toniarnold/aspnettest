using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using asplib.Controllers;

namespace test.asplib.Controllers
{
    [TestFixture]
    public class ControlStorageExtensionTest : IStorageController
    {
        public dynamic ViewBag { get; } // IStorageController

        [Test]
        public void GetViewstateNameTest()
        {
            Assert.That(this.GetStorageID(), Is.EqualTo("_CONTROLLER_ControlStorageExtensionTest"));
        }
    }
}
