using apicaller.Services;
using apiservice;
using asplib.Model.Db;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace apitest.apicaller.core.Services
{
    [TestFixture]
    public class ServiceClientTest : ServiceClient, IDeleteNewRows
    {
        #region scaffolding

        private TestServer _server;

        public List<(string, string, object)> MaxIds { get; set; } = new List<(string, string, object)>();

        /// <summary>
        /// "Foreign" connection string from the  service receiving API calls
        /// </summary>
        private string ConnectionString
        {
            get
            {
                return _configuration.GetConnectionString("ApiserviceDb");
            }
        }

        internal override Uri ServiceHost
        {
            get { return _server.BaseAddress; }
        }

        internal override HttpClient HttpClient
        {
            get { return _server.CreateClient(); }
        }

        [OneTimeSetUp]
        public void ConfigureServices()
        {
            _configuration = ServiceProvider.Configuration;
        }

        [OneTimeSetUp]
        public void ConfigureTestServer()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(_configuration)
                .UseStartup<Startup>();
            _server = new TestServer(builder);
        }

        [OneTimeTearDown]
        public void DisposeTestServer()
        {
            _server.Dispose();
        }

        [SetUp]
        public void GetAllMaxIds()
        {
            this.SelectMaxIds(ConnectionString, "Main", "mainid");
            this.SelectMaxIds(ConnectionString, "Accesscode", "accesscodeid");
        }

        [TearDown]
        public void DeleteNewRows()
        {
            this.DeleteMaxIdRows(ConnectionString);
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
            Assert.That(response, Is.EqualTo("ehlo"));
        }
    }
}