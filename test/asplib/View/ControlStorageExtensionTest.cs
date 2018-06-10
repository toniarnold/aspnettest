using asplib.View;
using NUnit.Framework;

namespace test.asplib.View
{
    [TestFixture]
    public class ControlStorageExtensionTest
    {
        [Test]
        public void StorageUninitializedTest()
        {
            Assert.That(ControlStorageExtension.SessionStorage, Is.Null);
        }

        [Test]
        public void EncryptDatabaseStorageUninitializedTest()
        {
            Assert.That(ControlStorageExtension.EncryptDatabaseStorage, Is.Null);
        }
    }
}