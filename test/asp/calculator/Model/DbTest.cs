using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using asp.calculator.Model;


namespace test.asp.calculator.Model
{
    [TestFixture]
    public class DbTest
    {
        [Test]
        public void SaveTest()
        {
            using (var db = new ASP_DBEntities())
            using (var trans = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                { 
                    // 1. Write
                    var session = Guid.NewGuid();
                    var now = DateTime.Now;

                    var main = new Main();
                    main.session = session;
                    main.created = now;
                    main.changed = now;
                    main.main = new byte[0];
                    db.Main.Add(main);
                    db.SaveChanges();

                    // 2. Read
                    var query = from m in db.Main
                                where m.session == session
                                select m;
                    var read = query.FirstOrDefault();

                    Assert.That(main.created, Is.EqualTo(now));
                }
                finally
                {
                    trans.Rollback();
                }
            }
        }
    }
}
