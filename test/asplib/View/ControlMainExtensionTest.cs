using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using asplib.View;

namespace test.asplib.View
{
    [TestFixture]
    public class ControlMainExtensionTest
    {
        [Test]
        public void StorageUninitializedTest()
        {
            Assert.That(ControlMainExtension.SessionStorage, Is.Null);
        }
    }
}
