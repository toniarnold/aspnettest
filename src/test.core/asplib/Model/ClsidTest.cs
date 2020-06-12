using asplib.Model;
using NUnit.Framework;
using System;

namespace test.asplib.Model
{
    [TestFixture]
    public class ClsidTest
    {
        [Clsid("33209477-886F-4031-8C34-8ACA964F1B96")]
        private class TestClass { }

        [Test]
        public void IdTest()
        {
            var obj = new TestClass();
            var id = Clsid.Id(obj);
            Assert.That(id, Is.EqualTo(Guid.Parse("33209477-886F-4031-8C34-8ACA964F1B96")));
        }

        [Test]
        public void MissingClsidTest()
        {
            var obj = new ClsidTest();
            Assert.That(() => Clsid.Id(obj), Throws.TypeOf<ArgumentException>());
        }
    }
}