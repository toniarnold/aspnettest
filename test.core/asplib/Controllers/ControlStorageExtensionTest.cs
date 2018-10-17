using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using asplib.Controllers;
using asplib.Model;
using Microsoft.Extensions.Configuration;

namespace test.asplib.Controllers
{
    [TestFixture]
    public class ControlStorageExtensionTest : IStorageController
    {
        // IStorageController
        public dynamic ViewBag { get; }
        public HttpContext HttpContext { get; }
        public IConfigurationRoot Configuration { get; }
        public Storage? SessionStorage { get; set; }

        [Test]
        public void GetViewstateNameTest()
        {
            Assert.That(this.GetStorageID(), Is.EqualTo("_CONTROLLER_ControlStorageExtensionTest"));
        }
    }
}
