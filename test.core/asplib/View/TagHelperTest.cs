using NUnit.Framework;
using static asplib.View.TagHelper;

namespace test.asplib.View
{
    /// <summary>
    /// C# unit tests for JavaScript functions
    /// </summary>
    [TestFixture]
    public class TagHelperTest
    {
        [Test]
        public void IdNoParentTest()
        {
            var id = Id("Enter");
            Assert.That(id, Is.EqualTo("Enter"));
        }

        [Test]
        public void IEmptyParentTest()
        {
            var id = Id(null, "Enter");
            Assert.That(id, Is.EqualTo("Enter"));
        }

        [Test]
        public void IdWithParentsTest()
        {
            var id = Id("Main", "Triptych", "Enter");
            Assert.That(id, Is.EqualTo("Main.Triptych.Enter"));
        }
    }
}