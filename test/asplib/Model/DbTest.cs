using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;

using NUnit.Framework;

using asplib.Model;


namespace test.asplib.Model
{
    [TestFixture]
    public class DbTest
    {
        [Test]
        public void SaveUpdateTest()
        {
            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                { 
                    var main = new Main();
                    db.Main.Add(main);  // add incomplete object, possible unlike with Strongly Typed Datasets
                    main.main = new byte[3] { 1, 2, 3 };

                    // INSERT: read back inserted and computed values
                    db.SaveChanges();
                    var insertedAt = DateTime.Now;
                    Assert.Multiple(() =>   // direct assertions on the model object
                    {
                        Assert.That(main.main, Is.EqualTo(new byte[3] { 1, 2, 3 }));
                        Assert.That(main.created, Is.EqualTo(main.changed));    // exact time from the db
                        Assert.That(main.changed, Is.EqualTo(insertedAt).Within(100).Milliseconds); // db rounttrip time
                    });

                    // UDPATE: read back updated and computed values
                    Thread.Sleep(500);  // greater than db roundtrip time
                    main.main = new byte[3] { 4, 5, 6 };
                    db.SaveChanges();
                    var createdAt = main.created;   // exact time from the db 
                    var changedAt = DateTime.Now;
                    Assert.Multiple(() =>   // direct assertions on the model object
                    {
                        Assert.That(main.main, Is.EqualTo(new byte[3] { 4, 5, 6 }));
                        Assert.That(main.created, Is.EqualTo(createdAt));
                        Assert.That(main.changed, Is.GreaterThan(main.created));
                        Assert.That(main.changed, Is.EqualTo(changedAt).Within(100).Milliseconds);  // db rounttrip time
                    });

                    // DELETE: delete with initially detached object
                    var session = main.session;
                    var detached = new Main { session = session };
                    db.Main.Attach(main);
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
            var query = from m in db.Main
                        where m.session == session
                        select m;
            var read = query.FirstOrDefault();
            Assert.That(read, Is.Null);
        }
    }
}
