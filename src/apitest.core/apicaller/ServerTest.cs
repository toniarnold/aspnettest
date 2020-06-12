using asplib.Model;
using asplib.Model.Db;
using asplib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using static Microsoft.Net.Http.Headers.HeaderNames;
using static System.Net.Mime.MediaTypeNames;
using ApicallerStartup = apicaller.Startup;
using ApiserviceServiceProvider = apitest.apiservice.ServiceProvider;
using ApiserviceStartup = apiservice.Startup;

namespace apitest.apicaller
{
    /// <summary>
    /// apicaller No assertions on IStaticController possible for the apiservice
    /// controller, as it is prodied through apicaller which overwrites the
    /// sinlge accessor, this no simple and succeeding AuthenticateVerifyTest()
    /// possible.
    /// </summary>
    [TestFixture]
    public class ServerTest : IDeleteNewRows
    {
        #region scaffolding

        private IConfiguration _configuration;

        private TestServer _callerServer;   // Our own server, directly used here

        private TestServer _serviceServer;  // Service server, called indirectly used from _callerServer

        private IHttpClientFactory _clientFactory;  // for our _callerServer

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
            // "Foreign" ApiserviceServer
            _serviceServer = CreateApiserviceServer(ApiserviceServiceProvider.Configuration);
            var serviceClientFactory = new TestServerClientFactory(_serviceServer);
            // "Our" ApicallerServer
            _configuration = ServiceProvider.Configuration;
            _callerServer = CreateApicallerServer(_configuration, serviceClientFactory);
            _clientFactory = new TestServerClientFactory(_callerServer);

            this.SelectMaxId(ConnectionString, "Accesscode", "accesscodeid");
            this.SelectMaxId(ConnectionString, "Main", "mainid");
        }

        [TearDown]
        public void DeleteNewRows()
        {
            this.DeleteNewRows(ConnectionString);
        }

        [OneTimeTearDown]
        public void DisposeTestServers()
        {
            _callerServer.Dispose();
            _serviceServer.Dispose();
        }

        /// <summary>
        /// "Our" TestServer with IHttpClientFactory injection for connecting to
        /// the "Foreign" TestServer
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="clientFactory"></param>
        /// <returns></returns>
        private TestServer CreateApicallerServer(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseStartup<ApicallerStartup>()
                .ConfigureTestServices(
                 services =>
                 {
                     // Inject  _serviceServer"s ClientFactory
                     services.AddSingleton(typeof(IHttpClientFactory), clientFactory);
                 });
            return new TestServer(builder);
        }

        private HttpClient GetHttpClient()
        {
            var client = _clientFactory.CreateClient("CallerClient");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(Application.Json));
            client.DefaultRequestHeaders.Add(UserAgent, ".NET HttpClient");
            return client;
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
            using (var client = GetHttpClient())
            {
                var response = client.GetStringAsync("/api/call/helo").Result;
                Assert.That(response, Is.EqualTo("\"ehlo\""));  // quoted due to "application/json" header
            }
        }

        [Test]
        public void AuthenticateTest()
        {
            using (var client = GetHttpClient())
            {
                var response = client.PostAsync("/api/call/authenticate", Json.Serialize(DbTestData.PHONENUMBER)).Result;
                var result = Json.Deserialize<string>(response.Content);
                Assert.That(result, Does.StartWith("Sent an SMS"));
                Assert.That(result, Does.Contain(DbTestData.PHONENUMBER));
            }
        }

        [Test]
        public void AuthenticateVerifyDeniedTest()
        {
            using (var client = GetHttpClient())
            {
                var responseAuth = client.PostAsync("/api/call/authenticate", Json.Serialize(DbTestData.PHONENUMBER)).Result;
                StorageImplementation.SetViewStateHeader(responseAuth, client);
                var resultAuth = Json.Deserialize<string>(responseAuth.Content);
                Assert.That(resultAuth, Does.StartWith("Sent an SMS"));
                Assert.That(resultAuth, Does.Contain(DbTestData.PHONENUMBER));

                for (int i = 0; i < 3; i++)
                {
                    var responseWrong = client.PostAsync("/api/call/verify", Json.Serialize("wrong code")).Result;
                    StorageImplementation.SetViewStateHeader(responseWrong, client);
                    var resultWrong = Json.Deserialize<string>(responseWrong.Content);
                    Assert.That(resultWrong, Does.StartWith("Wrong access code"));
                }

                var responseDenied = client.PostAsync("/api/call/verify", Json.Serialize("wrong code")).Result;
                StorageImplementation.SetViewStateHeader(responseDenied, client);
                var resultDeniedh = Json.Deserialize<string>(responseDenied.Content);
                Assert.That(resultDeniedh, Does.StartWith("Access denied"));
            }
        }
    }
}