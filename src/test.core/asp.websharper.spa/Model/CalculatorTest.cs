extern alias websharper_spa;
extern alias websharper_spa_Model;

using NUnit.Framework;
using websharper_spa_Model::asp.websharper.spa.Model;
using CalculatorContext = websharper_spa_Model::CalculatorContext;

namespace test.asp.websharper.spa.Model
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
        public void NewCalculatorTest()
        {
            var c = new Calculator();       // freshly instantiated calculator...
            Assert.That(c.State.Name, Is.EqualTo("Map1.Splash"));   // ...is in the init state
        }

        [Test]
        public void InitTest()
        {
            Assert.That(this.Fsm, Is.Not.Null);
            Assert.That(this.State.Name, Is.EqualTo("Map1.Splash"));                // "late binding"
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash));     // "early binding"
        }

        [Test]
        public void InitEnterTest()
        {
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Splash));
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
        }

        [Test]
        public void EnterTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("1");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("1"));
            Assert.That(this.Stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Fsm.Add(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ClrTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Fsm.Clr(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ClrAllTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Fsm.ClrAll(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void DivTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("12");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Fsm.Div(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void MulTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("4");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("3");
            var before = this.Stack.Count;
            this.Fsm.Mul(this.Stack);
            Assert.That(this.Stack.Peek(), Is.EqualTo("12"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void PowTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("2");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Fsm.Pow(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("4"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void SqrtTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("49");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Fsm.Sqrt(this.Stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("7"));
            Assert.That(this.Stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void SubTest()
        {
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("12");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Fsm.Enter("");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Fsm.Enter("3");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Fsm.Sub(this.stack);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            Assert.That(this.Stack.Peek(), Is.EqualTo("9"));
            Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
        }
    }
}