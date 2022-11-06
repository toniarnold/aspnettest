using asplib.Model;
using NUnit.Framework;
using System;

namespace test.asplib.Model
{
    [TestFixture]
    public class ViewModelTest
    {
        private const string TESTVALUE = "<some-string>";

        [Serializable]
        private class TestObj : IStored<TestObj>
        {
            public TestObj()
            {
                this.Embedded = TESTVALUE;
            }

            // Properties/Members of the concrete class Main:
            public string Embedded { get; set; }

            // IStored members not relevant here
            [NonSerialized] // not possible on auto property
            private ViewModel<TestObj> viewModel;

            public ViewModel<TestObj> ViewModel
            {
                get { return this.viewModel; }
                set { this.viewModel = value; }
            }

            public void Dispose()
            {
            }
        }

        private class StoredObj : ViewModel<TestObj>
        {
            public string Exposed;

            public override void LoadMembers()
            {
                this.Exposed = this.Main.Embedded;
            }

            public override void SaveMembers()
            {
                if (this.Exposed != null) // called as part of ToJson()
                {
                    this.Main.Embedded = this.Exposed;
                }
            }
        }

        [Test]
        public void InitializeEmptyTest()
        {
            var obj = new StoredObj();
            Assert.That(obj.Main, Is.Null);
            obj.DeserializeMain();      // implicitly new()
            Assert.That(obj.Main, Is.Not.Null);
            Assert.That(obj.Exposed, Is.Null);  // not yet copied
            obj.LoadMembers();
            Assert.That(obj.Exposed, Is.EqualTo(TESTVALUE));
        }

        [Test]
        public void SerializeMainDeserializeMainTest()
        {
            var obj = new StoredObj();
            Assert.That(obj.Main, Is.Null);
            obj.Main = new TestObj();   // instantiate Main explicitly
            obj.SerializeMain();
            obj.Main = null;        // delete Main
            obj.DeserializeMain();  // reconstruct Main from ViewState
            Assert.That(obj.Main, Is.Not.Null);
            obj.LoadMembers();      // expose the reconstructed members
            Assert.That(obj.Exposed, Is.EqualTo(TESTVALUE));
        }

        [Test]
        public void SerializeMainDeserializeMainModifyTest()
        {
            const string NEWVALUE = "<some new string>";

            var obj = new StoredObj();
            Assert.That(obj.Main, Is.Null);
            obj.Main = new TestObj();
            obj.SerializeMain();    // Main with the original values
            obj.Main = null;
            obj.Exposed = NEWVALUE; // modify an exposed value on the client without a Main instance
            obj.DeserializeMain();  // reconstruct Main with the old value from ViewState
            obj.SaveMembers();      // mirror the changed values without LoadMembers()
            Assert.That(obj.Main, Is.Not.Null);
            Assert.That(obj.Exposed, Is.EqualTo(NEWVALUE));         // set
            Assert.That(obj.Main.Embedded, Is.EqualTo(NEWVALUE));   // mirrored
        }

        // Non-JavaScript-Versions:

        [Test]
        public void JsonRoundTripTest()
        {
            var obj = new StoredObj();
            obj.Main = new TestObj();
            var json = obj.ToJson();
            Assert.That(json, Does.Not.Contain(TESTVALUE));     // binary serialized, hidden
            var copy = StoredObj.FromJson<StoredObj>(json);
            Assert.That(copy.Exposed, Is.EqualTo(TESTVALUE));   // restored
        }

        [Test]
        public void ArraySegmentRoundTripTest()
        {
            var obj = new StoredObj();
            obj.Main = new TestObj();
            var array = obj.ToArraySegment();
            var copy = ViewModel<TestObj>.FromArraySegment<StoredObj>(array);
            Assert.That(copy.Exposed, Is.EqualTo(TESTVALUE));
        }
    }
}