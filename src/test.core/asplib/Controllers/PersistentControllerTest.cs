using asplib.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace test.asplib.Controllers
{
    // Build a Controller class hierarchy to test recursive member serialization
    public class NonSerializable
    { }

    public class Controller1 : PersistentController
    {
        public string String1 { get; set; }

        [NonSerialized]
        private NonSerializable nonserialized = new NonSerializable();      // explicitly excluded

        private NonSerializable nonserializable = new NonSerializable();    // implicitly excluded
    }

    public class Controller2 : Controller1
    {
        protected string string2;
    }

    public class Controller3 : Controller2
    {
        public string String3;

        public string String2
        {
            get { return this.string2; }
            set { this.string2 = value; }
        }
    }

    [TestFixture]
    public class PersistentControllerTest : PersistentController
    {
        [Test]
        public void GetFieldsTest()
        {
            var obj = new Controller3();
            var members = new List<FieldInfo>();
            obj.GetFields(obj.GetType(), members);
            Assert.That(members.Where(m => m.Name == "nonserialized").Any(), Is.False);
            Assert.That(members.Where(m => m.Name == "nonserializable").Any(), Is.False);
        }

        [Test]
        public void GetValuesSetValuesTest()
        {
            var src = new Controller3();
            src.String1 = "String 1";
            src.String2 = "String 2";
            src.String3 = "String 3";
            var values = src.GetValues();

            var dst = new Controller3();
            dst.SetValues(values);

            Assert.Multiple(() =>
            {
                Assert.That(dst.String1, Is.EqualTo(src.String1));
                Assert.That(dst.String2, Is.EqualTo(src.String2));
                Assert.That(dst.String3, Is.EqualTo(src.String3));
            });
        }

        [Test]
        public void SerlalizeTest()
        {
            var obj = new Controller3();
            var bytes = obj.Serialize();  // throws if [NonSerialized] is not respected
        }

        [Test]
        public void IgnoreDeserializeErrorTest()
        {
            var bytes = new byte[] { 1, 2, 3 };
            this.Deserialize(bytes);
        }
    }
}