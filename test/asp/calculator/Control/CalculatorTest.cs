using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using asp.calculator.Control;


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
        public void AddTest()
        {
            this._fsm.Enter("");
            this.Enter("2");
            this._fsm.Enter("");
            this.Enter("3");
            var before = this.Stack.Count;
            this.Add();
            Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ClrTest()
        {
            this._fsm.Enter("");
            this.Enter("2");
            this._fsm.Enter("");
            this.Enter("3");
            var before = this.Stack.Count;
            this.Clr();
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ClrAllTest()
        {
            this._fsm.Enter("");
            this.Enter("2");
            this._fsm.Enter("");
            this.Enter("3");
            var before = this.Stack.Count;
            this.ClrAll();
            Assert.That(this.Stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void DivTest()
        {
            this._fsm.Enter("");
            this.Enter("12");
            this._fsm.Enter("");
            this.Enter("3");
            var before = this.Stack.Count;
            this.Div();
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void EnterTest()
        {
            this._fsm.Enter("");
            this.Enter("1");
            Assert.That(this.Stack.Peek(), Is.EqualTo("1"));
            Assert.That(this.Stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void MulTest()
        {
            this._fsm.Enter("");
            this.Enter("4");
            this._fsm.Enter("");
            this.Enter("3");
            var before = this.Stack.Count;
            this.Mul();
            Assert.That(this.Stack.Peek(), Is.EqualTo("12"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void PowTest()
        {
            this._fsm.Enter("");
            this.Enter("2");
            this._fsm.Enter("");
            var before = this.Stack.Count;
            this.Pow();
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void SqrtTest()
        {
            this._fsm.Enter("");
            this.Enter("49");
            var before = this.Stack.Count;
            this.Sqrt();
            Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void SubTest()
        {
            this._fsm.Enter("");
            this._fsm.Enter("12");
            this._fsm.Enter("");
            this._fsm.Enter("3");
            var before = this.Stack.Count;
            this._fsm.Sub(this._stack);
            Assert.That(this.Stack.Peek(), Is.EqualTo("9"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }
    }
}
