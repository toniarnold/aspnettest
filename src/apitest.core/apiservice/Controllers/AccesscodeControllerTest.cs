using apiservice.Controllers;
using apiservice.Model.Db;
using apiservice.Services;
using apiservice.View;
using asplib.Model.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using static AccesscodeContext.AuthMap;

namespace apitest.apiservice.Controllers
{
    /// <summary>
    /// Test the Controller methods of the AccesscodeController
    /// </summary>
    [TestFixture]
    [Category("Async")]
    [Category("ASP_DB")]
    public class AccesscodeControllerTest : AccesscodeController, IGlobalTransaction
    {
        private const string ACCESSCODE = "123456";

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
        public void BeginTrans()
        {
            this.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        [TearDown]
        public void RollbackTrans()
        {
            this.RollbackTransaction();
        }

        [SetUp]
        public void NewFSM()
        {
            base.Construct();
            this.Session = DbTestData.SESSION;
        }

        [TearDown]
        public void ResetCounter()
        {
            _attempts = 0;  // This is one of the reasons why xUnit re-instantiates the class for each test.
        }

        [Test]
        public void HeloTest()
        {
            var response = Helo().Result.Value;
            Assert.That(response, Is.EqualTo("ehlo"));
        }

        [Test]
        public void AuthenticateTest()
        {
            var query = new AuthenticateRequest()
            {
                Phonenumber = DbTestData.PHONENUMBER
            };
            var response = Authenticate(query).Result.Value;

            Assert.That(response.State, Is.EqualTo("AuthMap.Unverified"));
            Assert.That(response.Phonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
        }

        [Test]
        public void VerifySucceedTest()
        {
            _accesscode = ACCESSCODE;
            _pnonenumber = DbTestData.PHONENUMBER;
            this.State = Unverified;

            var query = new VerifyRequest()
            {
                Accesscode = ACCESSCODE
            };
            var response = Verify(query).Result.Value;

            Assert.That(response.State, Is.EqualTo("AuthMap.Verified"));
            Assert.That(response.Phonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
            Assert.That(response.Message, Does.Contain(DbTestData.PHONENUMBER));
        }

        [Test]
        public void VerifyFailTest()
        {
            _accesscode = ACCESSCODE;
            _pnonenumber = DbTestData.PHONENUMBER;
            this.State = Unverified;

            var query = new VerifyRequest()
            {
                Accesscode = "wrong"
            };
            var response = Verify(query).Result.Value;

            Assert.That(response.State, Is.EqualTo("AuthMap.Unverified"));
            Assert.That(response.Phonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
            Assert.That(response.Message, Does.Contain("retry"));
        }

        [Test]
        public void VerifyDeniedTest()
        {
            _accesscode = ACCESSCODE;
            _pnonenumber = DbTestData.PHONENUMBER;
            this.State = Unverified;

            var query = new VerifyRequest()
            {
                Accesscode = "wrong"
            };
            var fail1 = Verify(query).Result.Value;
            var fail2 = Verify(query).Result.Value;
            var fail3 = Verify(query).Result.Value;
            var response = Verify(query).Result.Value;

            Assert.That(response.State, Is.EqualTo("AuthMap.Denied"));
            Assert.That(response.Phonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
            Assert.That(response.Message, Does.Contain("denied"));
        }
    }
}