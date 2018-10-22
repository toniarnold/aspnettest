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
    [Serializable]
    public class ControlStorageExtensionTest : IStorageController
    {
        // IStorageController
        public dynamic ViewBag { get ; }
        public HttpContext HttpContext { get { return null; } }
        public IConfigurationRoot Configuration { get { return null; } }
        public Storage? SessionStorage { get { return null; } set { } }

        [Test]
        public void GetViewStateNameTest()
        {
            Assert.That(this.GetStorageID(), Is.EqualTo("_CONTROLLER_ControlStorageExtensionTest"));
        }

        [Test]
        public void GetSessionStorageNameTest()
        {
            Assert.That(this.GetSessionStorageID(), Is.EqualTo("_SESSIONSTORAGE_ControlStorageExtensionTest"));
        }

        private string TestProperty { get; set; }
        [Test]
        public void ViewStateTest()
        {
            this.TestProperty = "test value";
            var viewstate = this.ViewState();
            var fields = viewstate.Split(":");
            var bytes = Convert.FromBase64String(fields[1]);
            var copy = (ControlStorageExtensionTest)Serialization.Deserialize(bytes);
            Assert.That(copy.TestProperty, Is.EqualTo("test value"));
        }

        [Test]
        public void ViewStateEncryptedTest()
        {
            this.TestProperty = "test value";
            var secret = StorageControllerExtension.GetSecret("Key", "id");
            Func<byte[], byte[]> filter = x => Crypt.Encrypt(secret, x);
            var viewstate = this.ViewState(filter);
            var fields = viewstate.Split(":");
            var encrypted = Convert.FromBase64String(fields[1]);
            var bytes = Crypt.Decrypt(secret, encrypted);
            var copy = (ControlStorageExtensionTest)Serialization.Deserialize(bytes);
            Serialization.Deserialize(bytes);
            Assert.That(copy.TestProperty, Is.EqualTo("test value"));
        }
    }
}
