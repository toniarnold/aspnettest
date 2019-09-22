using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace apicaller.Services
{
    public class ServiceClient : IServiceClient
    {
        internal IConfiguration _configuration;
        internal IHttpClientFactory _clientFactory;

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
                await client.PostAsync(uri, content);
                return "OK";
            }
        }

        public async Task<ActionResult<string>> Verify(string accesscode)
        {
            using (var client = GetHttpClient())
            {
                var uri = ResouceUri("verifyPath");
                var content = new StringContent(accesscode);
                await client.PostAsync(uri, content);
                return "OK";
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
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, ".NET HttpClient");
        }
    }
}