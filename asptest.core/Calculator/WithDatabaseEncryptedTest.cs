using asplib.Controllers;
using asplib.Model;
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
            StorageImplementation.EncryptDatabaseStorage = true;
        }

        [OneTimeTearDown]
        public void ResetEncryption()
        {
            StorageImplementation.EncryptDatabaseStorage = null;
        }
    }
}