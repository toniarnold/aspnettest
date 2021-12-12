using asplib.Model;
using NUnit.Framework;

namespace test.asplib.Services
{
    [TestFixture]
    public class JsonTest
    {
        private class ObjectA
        {
            public string OnlyA { get; set; }
            public string Common { get; set; }
        }

        private class ObjectB
        {
            public string OnlyB { get; set; }
            public string Common { get; set; }
        }

        [Test]
        public void MapTest()
        {
            var src = new ObjectA()
            {
                OnlyA = "unmapped",
                Common = "Gets mapped"
            };

            var dst = Json.Map<ObjectB>(src);

            Assert.That(dst.Common, Is.EqualTo("Gets mapped"));
            Assert.That(dst.OnlyB, Is.Null);
        }

        [Test]
        public void SerializeDeserializeSameTest()
        {
            var src = new ObjectA()
            {
                OnlyA = "unmapped",
                Common = "Gets mapped"
            };

            var json = Json.Serialize(src);
            var copy = Json.Deserialize<ObjectA>(json);

            Assert.That(copy.OnlyA, Is.EqualTo("unmapped"));
            Assert.That(copy.Common, Is.EqualTo("Gets mapped"));
        }
    }
}