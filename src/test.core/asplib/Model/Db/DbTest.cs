using asplib.Model;
using asplib.Model.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Data;
using System.Threading;

namespace test.asplib.Model.Db
{
    [TestFixture]
    [Category("DbContext")]
    public class DbTest
    {
        [Clsid("00000000-0000-0000-0000-000000000000")]
        private class TestClass
        { }

        [OneTimeSetUp]
        public void SetUpDbContext()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            ASP_DBEntities.SetUpInsulatedDbContext(config["ASP_DBEntities"]);
        }

        [OneTimeTearDown]
        public void TearDownInstulatedDbContext()
        {
            ASP_DBEntities.TearDownInsulatedDbContext();
        }

        [Test]
        public void SaveUpdateTest()
        {
            const int DB_ROUNDTRIP_MILLISECONDS = 600;  // was only 200ms with the dotnet ef dbcontext scaffold version

            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var main = new Main();
                    db.Main.Add(main);  // add incomplete object, possible unlike with Strongly Typed Datasets
                    main.Clsid = new Guid();
                    main.Main1 = new byte[3] { 1, 2, 3 };

                    // INSERT: read back inserted and computed values
                    db.SaveChanges();
                    var insertedAt = DateTime.Now;
                    Assert.Multiple(() =>   // direct assertions on the model object
                    {
                        Assert.That(main.Main1, Is.EqualTo(new byte[3] { 1, 2, 3 }));
                        Assert.That(main.Created, Is.EqualTo(main.Changed));    // exact time from the db
                        Assert.That(main.Changed, Is.EqualTo(insertedAt).Within(DB_ROUNDTRIP_MILLISECONDS).Milliseconds,
                            "db round trip time");
                    });

                    // UDPATE: read back updated and computed values
                    Thread.Sleep(500);  // greater than db round-trip time
                    main.Main1 = new byte[3] { 4, 5, 6 };
                    db.SaveChanges();
                    var createdAt = main.Created;   // exact time from the db
                    var changedAt = DateTime.Now;
                    Assert.Multiple(() =>   // direct assertions on the model object
                    {
                        Assert.That(main.Main1, Is.EqualTo(new byte[3] { 4, 5, 6 }), "byte content is stored");
                        Assert.That(main.Created, Is.EqualTo(createdAt), "created date unchanged");
                        Assert.That(main.Changed, Is.GreaterThan(main.Created), "database trigger for updated changed date");
                        Assert.That(main.Changed, Is.EqualTo(changedAt).Within(DB_ROUNDTRIP_MILLISECONDS).Milliseconds,
                            "db round trip time"); ;
                    });

                    // DELETE: delete with initially detached object
                    var session = main.Session;
                    var detached = new Main { Session = session };
                    db.Main.Attach(main);   // must exist, otherwise SaveChanges() throws an exception
                    db.Main.Remove(main);
                    db.SaveChanges();
                    this.TestReadInexistent(db, session);
                }
                finally
                {
                    trans.Rollback();
                }
            }
        }

        [Test]
        public void ReadInexistentTest()
        {
            using (var db = new ASP_DBEntities())
            {
                this.TestReadInexistent(db, Guid.NewGuid());
            }
        }

        public void TestReadInexistent(ASP_DBEntities db, Guid session)
        {
            var read = db.LoadMain(session);
            Assert.That(read, Is.Null);
        }

        [Test]
        public void LoadSaveMainTest()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7 };

            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    // Local SetUp
                    var session = db.SaveMain(typeof(TestClass), bytes, null);

                    // Method under test
                    var copy = db.LoadMain(session);
                    Assert.That(copy, Is.EquivalentTo(bytes));
                }
                finally
                {
                    trans.Rollback();
                }
            }
        }

        // Unlike the WebForms variant, there is no LoadSaveMainFilteredTest(),
        // as the class is only responsible for raw data.

        [Test]
        public void InsertSQLTest()
        {
            var bytes = new byte[] { 1, 2, 3 };

            using (var db = new ASP_DBEntities())
            {
                var sql = db.InsertSQL(typeof(TestClass), bytes);
                Assert.That(sql, Does.Contain("INSERT INTO"));
            }
        }

        [Test]
        public void InsertSQLExecutabilityTest()
        {
            var bytes = new byte[] { 1, 2, 3 };

            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var sql = db.InsertSQL(typeof(TestClass), bytes);
                    db.Database.ExecuteSqlRaw(sql);
                    // really just executability without exception
                }
                finally
                {
                    trans.Rollback();
                }
            }
        }

        [Test]
        public void GetInstanceMissingTest()
        {
            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var none = db.LoadMain(Guid.NewGuid());
                Assert.That(none, Is.Null);
            }
        }
    }
}