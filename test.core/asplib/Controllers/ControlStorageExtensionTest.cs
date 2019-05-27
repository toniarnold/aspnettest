﻿using asplib.Controllers;
using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;

namespace test.asplib.Controllers
{
    [TestFixture]
    [Serializable]
    [Clsid("00000000-0000-0000-0000-000000000000")]
    public class ControlStorageExtensionTest : IStorageController
    {
        // IStorageController
        public dynamic ViewBag { get; }

        public HttpContext HttpContext { get { return null; } }
        public IConfigurationRoot Configuration { get { return null; } }
        public Storage? SessionStorage { get { return null; } set { } }
        public object Model { get { return null; } }

        [OneTimeSetUp]
        public void SetUpConnectionString()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            ASP_DBEntities.ConnectionString = config["ASP_DBEntities"];
        }

        [Test]
        public void GetViewStateNameTest()
        {
            Assert.That(this.GetStorageID(), Is.EqualTo("_STORAGEID_ControlStorageExtensionTest"));
        }

        [Test]
        public void GetSessionStorageNameTest()
        {
            Assert.That(this.GetSessionStorageID(), Is.EqualTo("_SESSIONSTORAGEID_ControlStorageExtensionTest"));
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
            var secret = StorageImplementation.GetSecret("Key");
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
