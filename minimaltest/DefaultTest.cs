﻿using iie;
using NUnit.Framework;
using System;

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
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
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