﻿using apiservice.Model.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Data;
using System.Linq;

namespace apitest.apiservice.Model.Db
{
    /// <summary>
    /// "Learning Tests" for an EF Core DbContext
    /// </summary>
    [Category("ASP_DB")]
    [TestFixture]
    public class AspserviceDbContextTest : AspserviceDbContext
    {
        /// <summary>
        /// Replaces the generated OnConfiguring in AspserviceDbContext
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ServiceProvider.Configuration.GetConnectionString("ApiserviceDb"));
            }
        }

        [Test]
        public void LoadSessionLinqTest()
        {
            var query = from m in Main
                        where m.Session == DbTestData.SESSION
                        select m;
            var main = query.FirstOrDefault();
            Assert.That(main, Is.Not.Null);
            Assert.That(main!.Session, Is.EqualTo(DbTestData.SESSION));
        }

        [Test]
        public void LoadSessionRawSQLTest()
        {
            var main = Main.FromSqlInterpolated($@"
                SELECT * FROM Main
                WHERE session = {DbTestData.SESSION}
                ").FirstOrDefault();
            Assert.That(main, Is.Not.Null);
            Assert.That(main!.Session, Is.EqualTo(DbTestData.SESSION));
        }

        [Test]
        public void TransactionTest()
        {
            using (var trans = this.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var testSession = Guid.NewGuid();
                try
                {
                    var main = new Main()
                    {
                        Session = testSession,
                        Clsid = Guid.NewGuid(),
                        Main1 = new byte[] { }
                    };
                    Main.Add(main);
                    SaveChanges();

                    var fromDb = LoadMain(testSession);
                    Assert.That(fromDb, Is.Not.Null);
                    Assert.That(fromDb!.Session, Is.EqualTo(testSession));
                }
                finally
                {
                    trans.Rollback();
                    var fromDb = LoadMain(testSession);
                    Assert.That(fromDb, Is.Null);
                }
            }
        }

        private Main? LoadMain(Guid session)
        {
            var query = from m in Main
                        where m.Session == session
                        select m;
            var inst = query.FirstOrDefault();
            return inst;
        }
    }
}