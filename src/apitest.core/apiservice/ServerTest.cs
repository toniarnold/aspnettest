using apiservice.Controllers;
using apiservice.View;
using asplib.Controllers;
using asplib.Model.Db;
using asplib.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using static AccesscodeContext.AuthMap;
using static Microsoft.Net.Http.Headers.HeaderNames;
using static System.Net.Mime.MediaTypeNames;

namespace apitest.apiservice
{
    /// <summary>
    /// apiservice
    /// </summary>
    [TestFixture]
    public class ServerTest : IDeleteNewRows
    {
        #region scaffolding

        // Instantiated by [OneTimeSetUp]:

        private IConfiguration _configuration = default!;

        private TestServer _server = default!;

        private IHttpClientFactory _clientFactory = default!;

        /// <summary>
        /// IDeleteNewRows
        /// </summary>
        public List<(string, string, object)> MaxIds { get; set; } = new();

        /// <summary>
        /// IDeleteNewRows
        /// The same connection string as for ServiceClientTest
        /// </summary>
        private string ConnectionString
        {
            get { return _configuration.GetConnectionString("ApiserviceDb"); }
        }

        private HttpClient GetHttpClient()
        {
            var client = _clientFactory.CreateClient("ServiceClient");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(Application.Json));
            client.DefaultRequestHeaders.Add(UserAgent, ".NET HttpClient");
            return client;
        }

        // IStaticController
        private AccesscodeController ServiceController
        {
            get { return (AccesscodeController)StaticControllerExtension.GetController(); }
        }

        [OneTimeSetUp]
        public void ConfigureServices()
        {
            _configuration = ServiceProvider.Configuration;
            _server = ServiceProvider.CreateTestServer();
            _clientFactory = new TestServerClientFactory(_server);

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
            using (var client = GetHttpClient())
            {
                var response = client.GetStringAsync("/api/accesscode/helo").Result;
                Assert.That(response, Is.EqualTo("\"ehlo\""));  // quoted due to "application/json" header
            }
        }

        [Test]
        public void AuthenticateTest()
        {
            using (var client = GetHttpClient())
            {
                var query = new AuthenticateRequest(DbTestData.PHONENUMBER);
                var response = client.PostAsync("/api/accesscode/authenticate", JsonContent.Serialize(query)).Result;
                var cookies = response.Headers.GetValues(SetCookie).ToArray();
                Assert.That(cookies, Has.Exactly(1).Items);
                var result = JsonContent.Deserialize<AuthenticateResponse>(response.Content);

                // Assertions on the result
                Assert.That(result, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(result!.State, Is.EqualTo("AuthMap.Unverified"));
                    Assert.That(result.Phonenumber, Is.EqualTo(DbTestData.PHONENUMBER));

                    // Assertions on IStaticController
                    Assert.That(ServiceController.State, Is.EqualTo(Unverified));
                    Assert.That(ServiceController._pnonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
                });
            }
        }

        [Test]
        public void AuthenticateVerifyTest()
        {
            using (var client = GetHttpClient())    // doessn't retain session cookies by itself
            {
                var queryAuth = new AuthenticateRequest(DbTestData.PHONENUMBER);
                var responseAuth = client.PostAsync("/api/accesscode/authenticate", JsonContent.Serialize(queryAuth)).Result;
                var cookies = responseAuth.Headers.GetValues(SetCookie).ToList();
                client.DefaultRequestHeaders.Add(Cookie, cookies[0]);   // set session

                var resultAuth = JsonContent.Deserialize<AuthenticateResponse>(responseAuth.Content);
                Assert.That(resultAuth, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(resultAuth!.State, Is.EqualTo("AuthMap.Unverified"));
                    Assert.That(resultAuth.Phonenumber, Is.EqualTo(DbTestData.PHONENUMBER));
                });

                // Get the correct access code that "leaks" through IStaticController:
                var accesscodeOk = ServiceController._accesscode;

                var queryVerifyWrong = new VerifyRequest("wrong code");
                var responseWrong = client.PostAsync("/api/accesscode/verify", JsonContent.Serialize(queryVerifyWrong)).Result;
                var resultWrong = JsonContent.Deserialize<AuthenticateResponse>(responseWrong.Content);
                Assert.That(resultWrong, Is.Not.Null);
                Assert.That(resultWrong!.State, Is.EqualTo("AuthMap.Unverified"));

                var queryVerifyOk = new VerifyRequest(accesscodeOk);
                var responseOk = client.PostAsync("/api/accesscode/verify", JsonContent.Serialize(queryVerifyOk)).Result;
                var resultOk = JsonContent.Deserialize<AuthenticateResponse>(responseOk.Content);
                Assert.That(resultOk, Is.Not.Null);
                Assert.That(resultOk!.State, Is.EqualTo("AuthMap.Verified"));
            }
        }

        [Test]
        public void AuthenticateVerifyDeniedTest()
        {
            using (var client = GetHttpClient())    // doessn't retain session cookies by itself
            {
                var queryAuth = new AuthenticateRequest(DbTestData.PHONENUMBER);
                var responseAuth = client.PostAsync("/api/accesscode/authenticate", JsonContent.Serialize(queryAuth)).Result;
                var cookies = responseAuth.Headers.GetValues(SetCookie).ToList();
                client.DefaultRequestHeaders.Add(Cookie, cookies[0]);   // set session

                var queryVerifyWrong = new VerifyRequest("wrong code");
                for (int i = 0; i < 3; i++)
                {
                    var responseWrong = client.PostAsync("/api/accesscode/verify", JsonContent.Serialize(queryVerifyWrong)).Result;
                    var resultWrong = JsonContent.Deserialize<AuthenticateResponse>(responseWrong.Content);
                    Assert.That(resultWrong, Is.Not.Null);
                    Assert.That(resultWrong!.State, Is.EqualTo("AuthMap.Unverified"));
                }

                var responseDenied = client.PostAsync("/api/accesscode/verify", JsonContent.Serialize(queryVerifyWrong)).Result;
                var resultDenied = JsonContent.Deserialize<AuthenticateResponse>(responseDenied.Content);
                Assert.That(resultDenied, Is.Not.Null);
                Assert.That(resultDenied!.State, Is.EqualTo("AuthMap.Denied"));
            }
        }
    }
}