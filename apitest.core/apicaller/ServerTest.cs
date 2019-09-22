using asplib.Model.Db;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
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

            this.SelectMaxIds(ConnectionString, "Main", "mainid");
            this.SelectMaxIds(ConnectionString, "Accesscode", "accesscodeid");
        }

        [TearDown]
        public void DeleteNewRows()
        {
            this.DeleteMaxIdRows(ConnectionString);
        }

        [OneTimeTearDown]
        public void DisposeTestServer()
        {
            _server.Dispose();
        }

        internal Uri ResouceUri(string fullpath)
        {
            Uri retval;
            var builder = new UriBuilder(_server.BaseAddress);
            Uri.TryCreate(builder.Uri, fullpath, out retval);
            return retval;
        }

        #endregion scaffolding

        [Test]
        public void HeloTest()
        {
            using (var client = _server.CreateClient())
            {
                var response = client.GetStringAsync("/api/call/helo").Result;
                Assert.That(response, Is.EqualTo("ehlo"));  // unquoted without reauest header
            }
        }
    }
}