using asplib.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace test.asplib.Model
{
    [TestFixture]
    public class SerializationTest
    {
        [Test]
        public void SerializeDeserializeTest()
        {
            var obj = new List<string> { "Hello", "World" };
            var bytes = Serialization.Serialize(obj);
            var copy = Serialization.Deserialize(bytes);
            Assert.That(copy, Is.EquivalentTo(obj));
        }

        [Test]
        public void SerializeDeserializeFilteredTest()
        {
            Func<byte[], byte[]> filter = x => { var y = (byte[])x.Clone(); Array.Reverse(y); return y; };
            var obj = new List<string> { "Hello", "World" };
            var bytes = Serialization.Serialize(obj, filter);
            var copy = Serialization.Deserialize(bytes, filter);
            Assert.That(copy, Is.EquivalentTo(obj));

            var failure = Serialization.Deserialize(bytes);
            Assert.That(failure, Is.Null);
        }

        [Test]
        public void DeserializeNullTest()
        {
            var none = Serialization.Deserialize(new byte[] { 1, 2, 3 });
            Assert.That(none, Is.Null);
        }
    }
}