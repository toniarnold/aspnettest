using asplib.View;
using NUnit.Framework;

namespace asptest.calculator
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
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