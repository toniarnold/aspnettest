using asplib.Controllers;
using NUnit.Framework;

namespace asptest.Calculator
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public class WithDatabaseEncryptedTest : WithDatabaseTest
    {
        [OneTimeSetUp]
        public void EnableEncryption()
        {
            StorageControllerExtension.EncryptDatabaseStorage = true;
        }

        [OneTimeTearDown]
        public void ResetEncryption()
        {
            StorageControllerExtension.EncryptDatabaseStorage = null;
        }
    }
}