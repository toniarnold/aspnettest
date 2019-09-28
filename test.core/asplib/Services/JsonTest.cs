using asplib.Services;
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
    }
}