using iie;
using NUnit.Framework;

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
            this.NavigateURL("about:blank", expectedStatusCode: 0);
            Assert.That(this.Html(), Is.EqualTo("<body></body>"));
        }
    }
}