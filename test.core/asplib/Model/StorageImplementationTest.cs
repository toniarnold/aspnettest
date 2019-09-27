using asplib.Model;
using asplib.Model.Db;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;

namespace test.asplib.Model
{
    [TestFixture]
    [Serializable]
    [Clsid("00000000-0000-0000-0000-000000000000")]
    public class StorageImplementationTest
    {
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
            Assert.That(StorageImplementation.GetStorageID(this.GetType().Name),
                Is.EqualTo("_STORAGEID_StorageImplementationTest"));
        }

        [Test]
        public void GetSessionStorageNameTest()
        {
            Assert.That(StorageImplementation.GetSessionStorageID(this.GetType().Name),
                Is.EqualTo("_SESSIONSTORAGEID_StorageImplementationTest"));
        }

        [Test]
        public void ViewStateTest()
        {
            string obj = "test value";
            var viewstate = StorageImplementation.ViewState(obj);
            var bytes = Convert.FromBase64String(viewstate);
            var copy = (string)Serialization.Deserialize(bytes);
            Assert.That(copy, Is.EqualTo(obj));
        }

        [Test]
        public void ViewStateEncryptedTest()
        {
            string obj = "test value";
            var secret = StorageImplementation.GetSecret("Key");
            Func<byte[], byte[]> filter = x => Crypt.Encrypt(secret, x);
            var viewstate = StorageImplementation.ViewState(obj, filter);
            var encrypted = Convert.FromBase64String(viewstate);
            var bytes = Crypt.Decrypt(secret, encrypted);
            var copy = (string)Serialization.Deserialize(bytes);
            Serialization.Deserialize(bytes);
            Assert.That(copy, Is.EqualTo("test value"));
        }

        [Test]
        public void LoadFromViewstateExistingTest()
        {
            string obj = "test value";
            var viewstate = StorageImplementation.ViewState(obj);
            var copy = StorageImplementation.LoadFromViewstate(() => new String("test value"), viewstate);
            Assert.That(copy, Is.EqualTo(obj));
        }

        [Test]
        public void LoadFromViewstateNewTest()
        {
            var copy = StorageImplementation.LoadFromViewstate(() => new String("test value"), null);
            Assert.That(copy, Is.EqualTo("test value"));
        }

        [Test]
        [Category("DbContext")]
        public void InsertSQLTest()
        {
            var retval = this.InsertSQL();
            Assert.That(retval, Does.Contain("INSERT INTO"));
        }
    }
}