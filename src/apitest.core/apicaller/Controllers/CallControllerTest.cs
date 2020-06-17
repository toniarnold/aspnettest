using apicaller.Controllers;
using apicaller.Services;
using apiservice.Controllers;
using asplib.Controllers;
using asplib.Model.Db;
using asplib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using static AccesscodeContext.AuthMap;
using ApiserviceServiceProvider = apitest.apiservice.ServiceProvider;
using ApiserviceStartup = apiservice.Startup;

namespace apitest.apicaller.Controllers
{
    /// <summary>
    /// Test the Controller methods of the allControlle
    /// </summary>
    [TestFixture]
    [Category("Async")]
    [Category("ASP_DB")]
    public class CallControllerTest : CallController, IDeleteNewRows
    {
        #region scaffolding

        /// <summary>
        /// IDeleteNewRows
        /// </summary>
        public List<(string, string, object)> MaxIds { get; set; }

        private TestServer _serviceServer;  // indirectly used

        /// <summary>
        /// IDeleteNewRows
        /// The same connection string as for ServiceClientTest
        /// </summary>
        private string ConnectionString
        {
            get { return this.configuration.GetConnectionString("ApiserviceDb"); }
        }

        // IStaticController of the server for the client possible here, as we're not running our own server
        private AccesscodeController ServiceController
        {
            get { return (AccesscodeController)StaticControllerExtension.GetController(); }
        }

        [OneTimeSetUp]
        public void ConfigureServices()
        {
            this.configuration = ServiceProvider.Configuration;
            _serviceServer = CreateApiserviceServer(ApiserviceServiceProvider.Configuration);
            var clientFactory = new TestServerClientFactory(_serviceServer);
            _serviceClient = new ServiceClient(this.configuration, clientFactory);

            this.SelectMaxId(ConnectionString, "Accesscode", "accesscodeid");
            this.SelectMaxId(ConnectionString, "Main", "mainid");
        }

        [TearDown]
        public void DeleteNewRows()
        {
            this.DeleteNewRows(ConnectionString);
        }

        /// <summary>
        /// "Foreign" TestServer
        /// </summary>
        /// <returns></returns>
        private TestServer CreateApiserviceServer(IConfiguration configuration)
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseStartup<ApiserviceStartup>();
            return new TestServer(builder);
        }

        #endregion scaffolding

        [Test]
        public void HeloTest()
        {
            var response = Helo().Result.Value;
            Assert.That(response, Is.EqualTo("ehlo"));
        }

        [Test]
        public void AuthenticateTest()
        {
            var response = Authenticate(DbTestData.PHONENUMBER).Result.Value;
            Assert.That(response, Does.StartWith("Sent an SMS"));
            Assert.That(response, Does.Contain(DbTestData.PHONENUMBER));
            Assert.That(response, Does.Contain(DbTestData.PHONENUMBER));
            Assert.That(ServiceController.State, Is.EqualTo(Unverified));
        }

        [Test]
        public void AuthenticateVerifyTest()
        {
            var response = Authenticate(DbTestData.PHONENUMBER).Result.Value;
            Assert.That(response, Does.StartWith("Sent an SMS"));
            Assert.That(response, Does.Contain(DbTestData.PHONENUMBER));
            Assert.That(ServiceController.State, Is.EqualTo(Unverified));
            var accesscodeOk = ServiceController._accesscode;

            var responseWrong = Verify("wrong code").Result.Value;
            Assert.That(responseWrong, Does.StartWith("Wrong access code"));
            Assert.That(ServiceController.State, Is.EqualTo(Unverified));

            var responseOk = this.Verify(accesscodeOk).Result.Value;
            Assert.That(responseOk, Does.StartWith("The phone number"));
            Assert.That(responseOk, Does.Contain(DbTestData.PHONENUMBER));
            Assert.That(ServiceController.State, Is.EqualTo(Verified));
        }

        [Test]
        public void AuthenticateVerifyDeniedTest()
        {
            var responseAuth = Authenticate(DbTestData.PHONENUMBER).Result.Value;
            Assert.That(responseAuth, Does.StartWith("Sent an SMS"));
            Assert.That(responseAuth, Does.Contain(DbTestData.PHONENUMBER));

            for (int i = 0; i < 3; i++)
            {
                var responseWrong = Verify("wrong code").Result.Value;
                Assert.That(responseWrong, Does.StartWith("Wrong access code"));
            }

            var responseDenied = Verify("wrong code").Result.Value;
            Assert.That(responseDenied, Does.StartWith("Access denied"));
        }
    }
}