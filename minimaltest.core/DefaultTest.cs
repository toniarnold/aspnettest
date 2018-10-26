using System;
using iie;
using NUnit.Framework;

namespace minimaltest
{
    /// <summary>
    /// No dependencies on the side of the web application itself except
    /// Application_EndRequest in Global.asax.cs, therefore the client id of
    /// the controls must be known in advance, as member name navigation cannot be used.
    /// Minimality here: Directly inherits from IIE, therefore an explicit
    /// [OneTimeSetUp]/[OneTimeTearDown] for SetUpIE()/TearDownIE() is required.
    /// </summary>
    [TestFixture]
    public class DefaultTest : IIE
    {
        [OneTimeSetUp]
        public void OneTimeSetUpIE()
        {
            this.SetUpIE();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownIE()
        {
            this.TearDownIE();
        }

        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
        }

        [Test]
        public void ClickWithStaticControllerTest()
        {
            this.Navigate("/");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
            this.ClickID("withstatic-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with static controller</h1>"));
        }

        [Test]
        public void ClickWithStorageControllerTest()
        {
            this.Navigate("/");
            this.ClickID("withstorage-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }

        //[Test]
        //[Ignore("eats up all resources available")]
        public void RecursiveTestTest()
        {
            this.Navigate("/");
            // This actually is evocative of Goethe's "The Sorcerer's Apprentice", as each
            // test run recursively runs the whole suite again, starting IE instances on the way.
            Assert.That(() => this.ClickID("testButton"), Throws.Exception.TypeOf<InvalidOperationException>());
        }
    }
}
