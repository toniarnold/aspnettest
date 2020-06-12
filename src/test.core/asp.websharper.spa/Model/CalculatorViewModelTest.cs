extern alias websharper_spa;

using NUnit.Framework;
using websharper_spa::asp.websharper.spa.Model;

namespace test.asp.websharper.spa.Model
{
    [TestFixture]
    public class CalculatorViewModelTest : CalculatorViewModel
    {
        [TearDown]
        public void TearDown()
        {
            this.Main = null;
            this.ViewState = null;
        }

        [Test]
        public void SerializeDeserializeMainTest()
        {
            // Verify empty object.
            Assert.That(this.Main, Is.Null);
            Assert.That(this.ViewState, Is.Null);

            // Create a new Calculator Task instance
            this.DeserializeMain();
            Assert.That(this.Main, Is.Not.Null);
            Assert.That(this.Main.Fsm.Owner, Is.EqualTo(this.Main));  // set by the constructor
            this.Main.Stack.Push("77"); // mutable data

            // delete it and recreate it from the ViewState
            this.SerializeMain();
            this.Main = null;
            this.DeserializeMain();
            Assert.That(this.Main, Is.Not.Null);
            Assert.That(this.Main.Stack.Peek(), Is.EqualTo("77"));
            Assert.That(this.Main.Fsm.Owner, Is.EqualTo(this.Main));  // not to be taken for granted
        }
    }
}