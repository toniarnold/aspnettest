using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using iie;

using asplib.View;


namespace testie.asp.calculator
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public class WithDatabaseEncryptedTest : WithDatabaseTest
    {
        [OneTimeSetUp]
        public void EnableEncryption()
        {
            ControlMainExtension.EncryptDatabaseStorage = true;
        }

        [OneTimeTearDown]
        public void ResetEncryption()
        {
            ControlMainExtension.EncryptDatabaseStorage = null;
        }
    }
}
