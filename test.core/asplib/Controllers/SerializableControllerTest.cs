using asplib.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace test.asplib.Controllers
{
    // Build a Controller class hierarchy to test recursive member serialization
    public class NonSerializable { }

    public class Controller1 : SerializableController
    {
        public string String1 { get; set; }

        [NonSerialized]
        private NonSerializable nonserializable = new NonSerializable();
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
    public class SerializableControllerTest : SerializableController
    {
        [Test]
        public void GetFieldsTest()
        {
            var obj = new Controller3();
            var members = new List<FieldInfo>();
            obj.GetFields(obj.GetType(), members);
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
                Assert.That(src.String1, Is.EqualTo(dst.String1));
                Assert.That(src.String2, Is.EqualTo(dst.String2));
                Assert.That(src.String3, Is.EqualTo(dst.String3));
            });
        }

        [Test]
        public void SerlalizeTest()
        {
            var obj = new Controller3();
            var bytes = obj.Serialize();  // throws if [NonSerialized] is not respected
        }
    }
}