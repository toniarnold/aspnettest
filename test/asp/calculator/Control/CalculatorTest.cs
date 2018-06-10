using asp.calculator.Control;
using NUnit.Framework;

namespace test.asp.calculator.Control
{
    [TestFixture]
    public class CalculatorTest : Calculator
    {
        [SetUp]
        public void SetUp()
        {
            base.Construct();
        }

        [Test]
        public void InitTest()
        {
            Assert.That(this._fsm, Is.Not.Null);
            Assert.That(this.State.Name, Is.EqualTo("Map1.Splash"));                // "late binding"
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash));     // "early binding"
        }

        [Test]
        public void InitEnterTest()
        {
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash));
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
        }

        [Test]
        public void EnterTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("1");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("1"));
            Assert.That(this.Stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this._fsm.Add(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ClrTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this._fsm.Clr(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ClrAllTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this._fsm.ClrAll(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void DivTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("12");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this._fsm.Div(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void MulTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("4");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("3");
            var before = this.Stack.Count;
            this._fsm.Mul(this.Stack);
            Assert.That(this.Stack.Peek(), Is.EqualTo("12"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void PowTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this._fsm.Pow(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void SqrtTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("49");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this._fsm.Sqrt(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void SubTest()
        {
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("12");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this._fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this._fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this._fsm.Sub(this._stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("9"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }
    }
}