using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using iie;

namespace test.iie
{
    /// <summary>
    /// Local smoke tests within the iie project
    /// </summary>
    [TestFixture]
    public class IIETest : IIE
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.SetUpIE();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.TearDownIE();
        }

        [Test]
        public void NavigateURLEmptyTest()
        {
            this.NavigateURL("about:blank");
            Assert.That(this.Html(), Is.EqualTo("<body></body>"));
        }
    }
}
