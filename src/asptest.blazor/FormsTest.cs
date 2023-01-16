using asp.blazor.Models;
using asp.blazor.Pages;
using asplib.Model;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Threading;

namespace asptest
{
    // The non-generic test fixture allows a class filter (unlike the generic one)
    //  "TestFilterWhere": "class==asptest.FormsTest"

    public class FormsTest : StaticComponentTest<EdgeDriver, Forms>
    {
        [SetUp]
        public void UnsetStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        [Test]
        public void NavigateFormTest()
        {
            Navigate("/forms");
            Find("h3").MarkupMatches("<h3>Forms</h3>");
        }

        [Test]
        public void SubmitEmptyFormTest()
        {
            Navigate("/forms");
            Click(Cut.submit);
            var validationErrrors = Find(".validation-errors");
            Assert.That(validationErrrors, Is.Not.Null);
            var messages = FindAll(".validation-message");
            Assert.That(messages, Has.Count.EqualTo(4));
            //Assert.That(Cut.editContext?.Validate(), Is.False); // wrong thread, only possible within the UI itself
        }

        [Test]
        public void SubmitValidFormTest()
        {
            Navigate("/forms");
            // Various methods to fill the elements
            Click(Cut.check, expectRenders: 0);
            Write(Cut.date, "15.5.2022");    // "2022-05-15" yields 2051-02-20
            Write(Cut.dec, "9876543.21");
            Write(Cut.integer, "256");
            // Single select -> will be Eggs
            Click(By.CssSelector, "#someSalad > option[value=Corn]", expectRenders: 0);
            Click(By.CssSelector, "#someSalad > option[value=Eggs]", expectRenders: 0);
            // Multiple select
            Click(By.CssSelector, "#saladSelection > option[value=Corn]", expectRenders: 0);
            Click(By.CssSelector, "#saladSelection > option[value=Lentils]", expectRenders: 0);
            Write(Cut.line, "one-liner");
            Write(Cut.paragraph, @"
                Line 1
                Line 2");

            Click(Cut.submit);

            Assert.Multiple(() =>
            {
                Assert.That(Cut.Main.Check, Is.True);
                Assert.That(Cut.Main.Date, Is.EqualTo(new DateTime(2022, 05, 15)));
                Assert.That(Cut.Main.Decimal, Is.EqualTo(9876543.21m));
                Assert.That(Cut.Main.Integer, Is.EqualTo(256));
                Assert.That(Cut.Main.SomeSalad, Is.EqualTo(Salad.Eggs));
                Assert.That(Cut.Main.SaladSelection, Has.Length.EqualTo(2));
                Assert.That(Cut.Main.Line, Is.EqualTo("one-liner"));
                Assert.That(Cut.Main.Paragraph, Is.EqualTo(@"
                Line 1
                Line 2".Replace("\r\n", "\n")));    // newlines from an input element seem to get delivered as "\n"
            });
        }
    }
}