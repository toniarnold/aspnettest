using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Microsoft.Net.Http.Headers.HeaderNames;
using static System.Net.Mime.MediaTypeNames;

namespace apicaller.Services
{
    public class ServiceClient : IServiceClient
    {
        internal IConfiguration _configuration;
        internal IHttpClientFactory _clientFactory;
        internal string[] _cookies;

        internal virtual Uri ServiceHost
        {
            get { return new Uri(_configuration.GetValue<string>("apiserviceHost")); }
        }

        internal HttpClient GetHttpClient()
        {
            var client = CreateHttpClient();
            AddDefaultHeaders(client);
            return client;
        }

        public ServiceClient()
        {
        }

        public ServiceClient(
            IConfiguration configuration,
            IHttpClientFactory clientFactory
            )
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<string> Helo()
        {
            using (var client = GetHttpClient())
            {
                return await client.GetStringAsync(this.ResouceUri("helo"));
            }
        }

        public async Task<ActionResult<string>> Authenticate(string phonenumber)
        {
            using (var client = GetHttpClient())
            {
                var uri = ResouceUri("authenticatePath");
                var content = new StringContent(phonenumber);
                var response = await client.PostAsync(uri, content);
                _cookies = response.Headers.GetValues(SetCookie).ToArray();
                return new OkResult();
            }
        }

        public async Task<ActionResult<string>> Verify(string accesscode)
        {
            using (var client = GetHttpClient())
            {
                var uri = ResouceUri("verifyPath");
                var content = new StringContent(accesscode);
                await client.PostAsync(uri, content);
                return new OkResult();
            }
        }

        internal Uri ResouceUri(string command)
        {
            Uri retval;
            var builder = new UriBuilder(ServiceHost);
            builder.Path = _configuration.GetValue<string>("authenticatePath");
            Uri.TryCreate(builder.Uri, command, out retval);
            return retval;
        }

        internal virtual HttpClient CreateHttpClient()
        {
            return _clientFactory.CreateClient();
        }

        internal void AddDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(Application.Json));
            client.DefaultRequestHeaders.Add(UserAgent, ".NET HttpClient");
            foreach (var cookie in _cookies ?? Enumerable.Empty<string>())
            {
                client.DefaultRequestHeaders.Add(Cookie, cookie);
            }
        }
    }
}