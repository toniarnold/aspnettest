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
        [Serializable]
        [Clsid("00000000-0000-0000-0000-000000000000")]
        private class TestClass : List<string> { }

        [Test]
        public void SetGetInstanceTest()
        {
            var obj = new TestClass { "Hello", "World" };
            this.SetInstance(obj);
            var copy = this.GetInstance<TestClass>();
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
                    var obj = new TestClass { "Hello", "World" };
                    var session = SaveMain(db, obj, null);

                    // Method under test
                    var copy = LoadMain<TestClass>(db, session);
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
        public void LoadSaveMainFilteredTest()
        {
            Func<byte[], byte[]> filter = x => (from b in x select (byte)~b).ToArray();

            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Local SetUp
                    var obj = new TestClass { "Hello", "World" };
                    var session = SaveMain(db, obj, null, filter);

                    // Method under test
                    var copy = LoadMain<TestClass>(db, session, filter);
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
        public void InsertSQLTest()
        {
            var obj = new TestClass { "Hello", "World" };
            this.SetInstance(obj);
            var sql = this.InsertSQL();
            Assert.That(sql, Does.Contain("INSERT INTO"));
        }

        [Test]
        [Category("DbContext")]
        public void InsertSQLExecutabilityTest()
        {
            var obj = new TestClass { "Hello", "World" };
            this.SetInstance(obj);
            var sql = this.InsertSQL();

            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var session = db.Database.SqlQuery<Guid>(sql).FirstOrDefault();
                    var copy = LoadMain<TestClass>(db, session);
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
                    var before = AllMainRows<TestClass>(db).Count();
                    var obj = new TestClass { "Hello", "World" };
                    var session1 = SaveMain(db, obj, null);
                    var session2 = SaveMain(db, obj, null);

                    // Method under test
                    var all = AllMainRows<TestClass>(db);
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
                        Assert.That(found1.GetInstance<TestClass>(), Is.EquivalentTo(obj));
                        Assert.That(found2.GetInstance<TestClass>(), Is.EquivalentTo(obj));
                        // serialize & deserialize creates a copy of the object
                        Assert.That(found1.GetInstance<TestClass>(),
                            Is.Not.SameAs(found2.GetInstance<TestClass>()));
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
                var all = AllMainRows<TestClass>(db);
                var count = all.Count();
                Console.WriteLine(String.Format("all.Count() = {0}", count));
            }
        }
    }
}