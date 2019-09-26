using apiservice.Controllers;
using apiservice.Model.Db;
using apiservice.Services;
using asplib.Model.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static AccesscodeContext.AuthMap;

namespace apitest.apiservice.Controllers
{
    /// <summary>
    /// Test the FSM methods of the AccesscodeController
    /// </summary>
    [TestFixture]
    public class AccesscodeFsmTest : AccesscodeController, IGlobalTransaction
    {
        public List<DbContext> DbContexts { get; set; }

        [OneTimeSetUp]
        public void ConfigureServices()
        {
            _configuration = ServiceProvider.Configuration;
            _DbContext = ServiceProvider.Get<AspserviceDbContext>();
            _SMSService = ServiceProvider.Get<ISMSService>();
            this.RetrieveDbContexts();
        }

        [SetUp]
        public void NewFSM()
        {
            base.Construct();
            this.Session = DbTestData.SESSION;
            this.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        [TearDown]
        public void RollbackTrans()
        {
            this.RollbackTransaction();
        }

        [TearDown]
        public void ResetCounter()
        {
            _attempts = 0;  // This is one of the reasons why xUnit re-instantiates the class for each test.
        }

        [Test]
        public void InitTest()
        {
            Assert.That(this.Fsm, Is.Not.Null);
            Assert.That(this.State.Name, Is.EqualTo("AuthMap.Idle"));
            Assert.That(this.State, Is.EqualTo(Idle));
        }

        [Test]
        public void AuthenticateTest()
        {
            const string PHONE = "555012345";
            Assert.That(this.State, Is.EqualTo(Idle));
            this.Fsm.Authenticate(PHONE);
            Assert.That(this.State, Is.EqualTo(Unverified));
            Assert.That(_pnonenumber, Is.EqualTo(PHONE));
        }

        [Test]
        public void AuthenticateVerifyTest()
        {
            Assert.That(this.State, Is.EqualTo(Idle));
            this.Fsm.Authenticate(DbTestData.PHONENUMBER);
            Assert.That(this.State, Is.EqualTo(Unverified));
            Assert.That(_pnonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
            this.Fsm.Verify("wrong code");
            Assert.That(this.State, Is.EqualTo(Unverified));
            this.Fsm.Verify(_accesscode);   // correct code
            Assert.That(this.State, Is.EqualTo(Verified));
        }

        [Test]
        public void AuthenticateVerifyDeniedTest()
        {
            Assert.That(this.State, Is.EqualTo(Idle));
            this.Fsm.Authenticate(DbTestData.PHONENUMBER);
            Assert.That(this.State, Is.EqualTo(Unverified));
            Assert.That(_pnonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
            for (int i = 0; i < 3; i++)
            {
                this.Fsm.Verify("wrong code");
            }
            this.Fsm.Verify("4");
            Assert.That(this.State, Is.EqualTo(Denied));
        }

        [Test]
        public void SaveAccesscodeTest()
        {
            // local setup: fields to store
            const string PHONE = "555031415926";
            const string CODE = "716253";
            _pnonenumber = PHONE;
            _accesscode = CODE;

            // method under test
            SaveAccesscode();

            // retrieve the stored entity for assertions
            var stored = (from a in _DbContext.Accesscode
                          where a.Phonenumber == PHONE
                          select a).FirstOrDefault();
            Assert.That(stored, Is.Not.Null);
            Assert.That(stored.Accesscode1, Is.EqualTo(CODE));
        }
    }
}