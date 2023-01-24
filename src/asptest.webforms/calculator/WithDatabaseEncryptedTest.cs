using asplib.View;
using NUnit.Framework;

namespace asptest.calculator
{
    [TestFixture]
    public class WithDatabaseEncryptedTest : WithDatabaseTest
    {
        [OneTimeSetUp]
        public void EnableEncryption()
        {
            ControlStorageExtension.EncryptDatabaseStorage = true;
        }

        [OneTimeTearDown]
        public void ResetEncryption()
        {
            ControlStorageExtension.EncryptDatabaseStorage = null;
        }
    }
}