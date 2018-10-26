using asplib.Common;
using NUnit.Framework;
using System.Collections.Specialized;

namespace test.asplib.Common
{
    [TestFixture]
    public class CookieSubkeyExtensionTest
    {
        [Test]
        public void GetSetSubkeyTest()
        {
            var dict = new NameValueCollection();
            dict["a=b&c=d"] = "A=B&C=D";    // needs to be escaped
            var single = dict.ToCookieString();
            var clone = single.FromCookieString();
            Assert.That(clone, Is.EquivalentTo(dict));
        }

        [Test]
        public void FromCookieStringEmptyTest()
        {
            string cookie = null;
            var dict = cookie.FromCookieString();
            Assert.That(dict, Is.Empty);
        }
    }
}