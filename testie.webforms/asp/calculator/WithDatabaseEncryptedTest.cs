using asplib.View;
using NUnit.Framework;

namespace testie.asp.calculator
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