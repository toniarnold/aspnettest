using apiservice.Model;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace apitest.apiservice.Model
{
    [TestFixture]
    public class AccesscodeGeneratorTest : AccesscodeGenerator
    {
        private const int LENGTH = 6;

        [Test]
        public void NewTest()
        {
            var retval = New(LENGTH);
            Assert.That(retval.Length, Is.EqualTo(LENGTH));
            Assert.That(Regex.IsMatch(retval, @"^\d\d\d\d\d\d$"), Is.True);
        }
    }
}