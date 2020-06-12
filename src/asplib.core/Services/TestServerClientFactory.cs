using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace asplib.Services
{
    public class TestServerClientFactory : IHttpClientFactory
    {
        private readonly TestServer _server;

        public TestServerClientFactory(TestServer server)
        {
            _server = server;
        }

        /// <summary>
        /// Return the anonymous Client from the underlying TestServer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HttpClient CreateClient(string name) => _server.CreateClient();
    }
}