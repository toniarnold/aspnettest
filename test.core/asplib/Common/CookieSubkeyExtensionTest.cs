using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using asplib.Common;
using NUnit.Framework;
using Microsoft.AspNetCore.Http.Internal;

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
        public void FromCookieStringNullTest()
        {
            string cookie = null;
            var dict = cookie.FromCookieString();
            Assert.That(dict, Is.Null);
        }
    }
}
