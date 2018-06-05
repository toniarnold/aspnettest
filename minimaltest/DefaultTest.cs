using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using iie;

namespace minimaltest
{
    /// <summary>
    /// No dependencies on the side of the web application itself except
    /// Application_EndRequest in Global.asax.cs, therefore the client id of
    /// the controls must be known in advance, as member name navigation cannot be used.
    /// </summary>
    [TestFixture]
    public class DefaultTest : IIE
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
        public void NavigateDefaultTest()
        {
            this.Navigate("/minimal/default.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
        }

        [Test]
        public void ClickWithrootTest()
        {
            this.Navigate("/minimal/default.aspx");
            this.ClickID("withroot-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with root</h1>"));
        }

        [Test]
        public void ClickWithstorageTest()
        {
            this.Navigate("/minimal/default.aspx");
            this.ClickID("withstorage-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }

        [Test]
        public void ClickControlThrowsTest()
        {
            this.Navigate("/minimal/default.aspx");
            // If this would work here, it would be evocative of Goethe's "The Sorcerer's Apprentice"...
            Assert.That(() => this.Click("testButton"), Throws.Exception.TypeOf<InvalidOperationException>());
        }

    }
}
