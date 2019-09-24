using asplib.Model.Db;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace apitest.apicaller
{
    /// <summary>
    /// apicaller
    /// </summary>
    [TestFixture]
    public class ServerTest : IDeleteNewRows
    {
        #region scaffolding

        private IConfiguration _configuration;

        private TestServer _server;

        /// <summary>
        /// IDeleteNewRows
        /// </summary>
        public List<(string, string, object)> MaxIds { get; set; }

        /// <summary>
        /// IDeleteNewRows
        /// The same connection string as for ServiceClientTest
        /// </summary>
        private string ConnectionString
        {
            get { return _configuration.GetConnectionString("ApiserviceDb"); }
        }

        [OneTimeSetUp]
        public void ConfigureServices()
        {
            _configuration = ServiceProvider.Configuration;
            _server = ServiceProvider.CreateTestServer();

            this.SelectMaxId(ConnectionString, "Accesscode", "accesscodeid");
            this.SelectMaxId(ConnectionString, "Main", "mainid");
        }

        [TearDown]
        public void DeleteNewRows()
        {
            this.DeleteNewRows(ConnectionString);
        }

        [OneTimeTearDown]
        public void DisposeTestServer()
        {
            _server.Dispose();
        }

        #endregion scaffolding

        [Test]
        public void HeloTest()
        {
            using (var client = _server.CreateClient())
            {
                var response = client.GetStringAsync("/api/call/helo").Result;
                Assert.That(response, Is.EqualTo("ehlo"));  // unquoted without request header
            }
        }
    }
}