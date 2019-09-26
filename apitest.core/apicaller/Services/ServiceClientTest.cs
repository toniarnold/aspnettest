using apicaller.Services;
using apiservice;
using apiservice.Controllers;
using asplib.Controllers;
using asplib.Model.Db;
using asplib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static AccesscodeContext.AuthMap;
using ApiserviceServiceProvider = apitest.apiservice.ServiceProvider;

namespace apitest.apicaller.core.Services
{
    /// <summary>
    /// ServiceClient calls /authenticate/-methods on the apiservice.core
    /// server, therefore the TestServer is created with the startup from the
    /// apiservice.core project.
    /// </summary>
    [TestFixture]
    public class ServiceClientTest : ServiceClient, IDeleteNewRows
    {
        #region scaffolding

        private TestServer _server;

        /// <summary>
        /// IDeleteNewRows
        /// </summary>
        public List<(string, string, object)> MaxIds { get; set; }

        /// <summary>
        /// "Foreign" connection string from the  service receiving the API calls
        /// </summary>
        private string ConnectionString
        {
            get { return _configuration.GetConnectionString("ApiserviceDb"); }
        }

        internal override Uri ServiceHost
        {
            get { return _server.BaseAddress; }
        }

        // IStaticController of the server for the client
        private AccesscodeController ServiceController
        {
            get { return (AccesscodeController)StaticControllerExtension.GetController(); }
        }

        [OneTimeSetUp]
        public void ConfigureServices()
        {
            _configuration = ServiceProvider.Configuration;
            // Use not our own (apicaller) configuration, but the one from the apiservice server:
            _server = CreateApiserviceServer(ApiserviceServiceProvider.Configuration);
            _clientFactory = new TestServerClientFactory(_server);

            this.SelectMaxId(ConnectionString, "Accesscode", "accesscodeid");
            this.SelectMaxId(ConnectionString, "Main", "mainid");
        }

        [TearDown]
        public void DeleteNewRows()
        {
            this.DeleteNewRows(ConnectionString);
        }

        [TearDown]
        public void DeleteSession()
        {
            Cookies = null;
        }

        [OneTimeTearDown]
        public void DisposeTestServer()
        {
            _server.Dispose();
        }

        /// <summary>
        /// "Foreign" TestServer for the client
        /// </summary>
        /// <returns></returns>
        private TestServer CreateApiserviceServer(IConfiguration configuration)
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
            return new TestServer(builder);
        }

        #endregion scaffolding

        [Test]
        public void ResourceUriTest()
        {
            var uri = this.ResouceUri("helo");
            // TestServer on localhost without SSL
            Assert.That(uri, Is.EqualTo(new Uri("http://localhost/api/accesscode/helo")));
        }

        [Test]
        public void HeloTest()
        {
            var response = this.Helo().Result;
            Assert.That(response, Is.EqualTo("\"ehlo\""));  // quoted due to "application/json" header
        }

        [Test]
        public void AuthenticateTest()
        {
            using (var client = GetHttpClient())
            {
                var response = this.Authenticate(DbTestData.PHONENUMBER).Result;
                Assert.That(response.Value, Does.StartWith("Sent an SMS"));
                Assert.That(response.Value, Does.Contain(DbTestData.PHONENUMBER));
                Assert.That(Cookies, Is.Not.Null); // created as session
                Assert.That(Cookies, Has.Exactly(1).Items);
                // Assertions on IStaticController
                Assert.That(ServiceController.State, Is.EqualTo(Unverified));
                Assert.That(ServiceController._pnonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
            }
        }

        [Test]
        public void AuthenticateVerifyTest()
        {
            using (var client = GetHttpClient())
            {
                var responseAuth = this.Authenticate(DbTestData.PHONENUMBER).Result;
                Assert.That(responseAuth.Value, Does.StartWith("Sent an SMS"));
                Assert.That(responseAuth.Value, Does.Contain(DbTestData.PHONENUMBER));
                Assert.That(ServiceController.State, Is.EqualTo(Unverified));
                var accesscodeOk = ServiceController._accesscode;

                var responseWrong = this.Verify("wrong code").Result;
                Assert.That(responseWrong.Value, Does.StartWith("Wrong access code"));
                Assert.That(ServiceController.State, Is.EqualTo(Unverified));

                var responseOk = this.Verify(accesscodeOk).Result;
                Assert.That(responseOk.Value, Does.StartWith("The phone number"));
                Assert.That(responseAuth.Value, Does.Contain(DbTestData.PHONENUMBER));
                Assert.That(ServiceController.State, Is.EqualTo(Verified));
            }
        }

        [Test]
        public void AuthenticateVerifyDeniedTest()
        {
            using (var client = GetHttpClient())
            {
                var responseAuth = this.Authenticate(DbTestData.PHONENUMBER).Result;
                Assert.That(responseAuth.Value, Does.StartWith("Sent an SMS"));
                Assert.That(responseAuth.Value, Does.Contain(DbTestData.PHONENUMBER));
                Assert.That(ServiceController.State, Is.EqualTo(Unverified));
                var accesscodeOk = ServiceController._accesscode;

                for (int i = 0; i < 3; i++)
                {
                    var responseWrong = this.Verify("wrong code").Result;
                    Assert.That(responseWrong.Value, Does.StartWith("Wrong access code"));
                    Assert.That(ServiceController.State, Is.EqualTo(Unverified));
                }

                var responseDenied = this.Verify("wrong code").Result;
                Assert.That(responseDenied.Value, Does.StartWith("Access denied"));
                Assert.That(ServiceController.State, Is.EqualTo(Denied));
            }
        }
    }
}