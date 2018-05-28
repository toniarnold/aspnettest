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

        [Test]
        public void SerializeDeserializeTest()
        {
            var obj = new List<string> { "Hello", "World" };
            var bytes = ControlMainExtension.Serialize(obj);
            var copy = ControlMainExtension.Deserialize(bytes);
            Assert.That(copy, Is.EquivalentTo(obj));
        }
    }
}
