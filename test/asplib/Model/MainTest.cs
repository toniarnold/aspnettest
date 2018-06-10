using asplib.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace test.asplib.Model
{
    [TestFixture]
    public class MainTest : Main
    {
        [Test]
        public void SerializeDeserializeTest()
        {
            var obj = new List<string> { "Hello", "World" };
            var bytes = this.Serialize(obj);
            var copy = this.Deserialize(bytes);
            Assert.That(copy, Is.EquivalentTo(obj));
        }

        [Test]
        public void SerializeDeserializeFilteredTest()
        {
            Func<byte[], byte[]> filter = x => { var y = (byte[])x.Clone(); Array.Reverse(y); return y; };
            var obj = new List<string> { "Hello", "World" };
            var bytes = this.Serialize(obj, filter);
            var copy = this.Deserialize(bytes, filter);
            Assert.That(copy, Is.EquivalentTo(obj));

            var failure = this.Deserialize(bytes);
            Assert.That(failure, Is.Null);
        }

        [Test]
        public void DeserializeNullTest()
        {
            var none = this.Deserialize(new byte[] { 1, 2, 3 });
            Assert.That(none, Is.Null);
        }

        [Test]
        public void SetGetInstanceTest()
        {
            var obj = new List<string> { "Hello", "World" };
            this.SetInstance(obj);
            var copy = this.GetInstance<List<string>>();
            Assert.That(copy, Is.EquivalentTo(obj));

            var none = this.GetInstance<List<int>>();
            Assert.That(none, Is.Null);
        }

        [Test]
        [Category("DbContext")]
        public void LoadSaveMainTest()
        {
            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Local SetUp
                    var obj = new List<string> { "Hello", "World" };
                    var session = SaveMain(db, obj, null);

                    // Method under test
                    var copy = LoadMain<List<string>>(db, session);
                    Assert.That(copy, Is.EquivalentTo(obj));
                }
                finally
                {
                    trans.Rollback();
                }
            }
        }

        [Test]
        [Category("DbContext")]
        public void GetInstanceMissingTest()
        {
            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var none = LoadMain<object>(Guid.NewGuid());
                Assert.That(none, Is.Null);
            }
        }

        [Test]
        [Category("DbContext")]
        public void AllMainRowsInsertTest()
        {
            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Local SetUp
                    var before = AllMainRows<List<string>>(db).Count();
                    var obj = new List<string> { "Hello", "World" };
                    var session1 = SaveMain(db, obj, null);
                    var session2 = SaveMain(db, obj, null);

                    // Method under test
                    var all = AllMainRows<List<string>>(db);
                    Assert.That(all.Count(), Is.EqualTo(before + 2));
                    Main found1 = null;
                    Main found2 = null;
                    foreach (var main in all)
                    {
                        if (main.session == session1)
                        {
                            found1 = main;
                        }
                        else if (main.session == session2)
                        {
                            found2 = main;
                        }
                    }
                    Assert.Multiple(() =>
                    {
                        Assert.That(found1, Is.Not.Null);
                        Assert.That(found2, Is.Not.Null);
                        Assert.That(found1.GetInstance<List<string>>(), Is.EquivalentTo(obj));
                        Assert.That(found2.GetInstance<List<string>>(), Is.EquivalentTo(obj));
                        // serialize & deserialize creates a copy of the object
                        Assert.That(found1.GetInstance<List<string>>(),
                            Is.Not.SameAs(found2.GetInstance<List<string>>()));
                    });
                }
                finally
                {
                    trans.Rollback();   // trans.Commit();
                }
            }
        }

        [Test]
        [Category("DbContext")]
        public void AllMainRowsTest()
        {
            // use  trans.Commit() above to have some entries of the correct type
            using (var db = new ASP_DBEntities())
            {
                var all = AllMainRows<List<string>>(db);
                var count = all.Count();
                Console.WriteLine(String.Format("all.Count() = {0}", count));
            }
        }
    }
}