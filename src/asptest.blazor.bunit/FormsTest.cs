using asp.blazor.Models;
using asp.blazor.Pages;
using asplib.Services;
using Bunit;
using iselenium;

namespace asptest.blazor.bunit
{
    public class FormsTest : BUnitTestContext
    {
        [OneTimeSetUp]
        public void RegisterModel()
        {
            Services.AddPersistent<FormsModel>();
        }

        [Test]
        public void InstantiateFormsTest()
        {
            var cut = RenderComponent<Forms>();
            cut.Find("h3").MarkupMatches("<h3>Forms</h3>");
        }

        [Test]
        public void SubmitEmptyFormsTest()
        {
            var cut = RenderComponent<Forms>();
            cut.Find(cut.Instance.submit).Click();
            var validationErrrors = cut.Find(".validation-errors");
            Assert.That(validationErrrors, Is.Not.Null);
            var messages = cut.FindAll(".validation-message");
            Assert.Multiple(() =>
            {
                Assert.That(messages, Has.Count.EqualTo(4));
                Assert.That(validationErrrors.ChildNodes, Has.Length.EqualTo(4));   // children not available in IWebElement
            });
        }

        [Test]
        public void SubmitValidFormTest()
        {
            var cut = RenderComponent<Forms>();
            // Various methods to fill the elements
            //cut.Find(cut.Instance.check).Click();   // Bunit.MissingEventHandlerException : The element does not have an event handler for the event 'onclick'. It does however have an event handler for the 'onchange' event.
            cut.Find(cut.Instance.check).Change(true);
            // ReferenceCaptureId deleted now, fall back to #id selector, see: https://github.com/bUnit-dev/bUnit/issues/7
            cut.Find("#date").Change("2022-05-15");    // Unlike with iSelenium, "15.5.2022" is not recognized
            cut.Find("#decimal").Change("9876543.21");
            cut.Find("#integer").Change("256");
            // Single select -> will be Eggs
            cut.Find("#someSalad").Change("Corn");
            cut.Find("#someSalad").Change("Eggs");
            // Multiple select does not work the same way in current bUnit 1.9.8, the SaladSelection assertion fails, as nothing gets selected.
            //cut.Find("#saladSelection > option[value=Corn]").Click(); // Bunit.MissingEventHandlerException : The element does not have an event handler for the event 'onclick'. It does however have an event handler for the 'onchange' event.
            cut.Find("#saladSelection").Change("Corn");
            cut.Find("#saladSelection").Change("Lentils");
            cut.Find("#line").Change("one-liner");
            cut.Find("#paragraph").Change(@"
                Line 1
                Line 2");

            cut.Find("#submit").Click();
            var validationErrrors = cut.Find(".validation-errors");

            Assert.Multiple(() =>
            {
                Assert.That(cut.Instance.Main.Check, Is.True);
                Assert.That(cut.Instance.Main.Date, Is.EqualTo(new DateTime(2022, 05, 15)));
                Assert.That(cut.Instance.Main.Decimal, Is.EqualTo(9876543.21m));
                Assert.That(cut.Instance.Main.Integer, Is.EqualTo(256));
                Assert.That(cut.Instance.Main.SomeSalad, Is.EqualTo(Salad.Eggs));
                Assert.That(cut.Instance.Main.SaladSelection, Has.Length.EqualTo(2));
                Assert.That(cut.Instance.Main.Line, Is.EqualTo("one-liner"));
                Assert.That(cut.Instance.Main.Paragraph, Is.EqualTo(@"
                Line 1
                Line 2"));    // newlines remain as they are
            });
        }
    }
}