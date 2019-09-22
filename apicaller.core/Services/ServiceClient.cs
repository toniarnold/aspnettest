using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace apicaller.Services
{
    public class ServiceClient : IServiceClient
    {
        internal IConfiguration _configuration;
        internal IHttpClientFactory _clientFactory;
        internal HttpClient _httpClient;

        internal virtual Uri ServiceHost
        {
            get { return new Uri(_configuration.GetValue<string>("apiserviceHost")); }
        }

        internal virtual HttpClient HttpClient
        {
            get { return _clientFactory.CreateClient(); }
        }

        public ServiceClient()
        {
        }

        public ServiceClient(
            IConfiguration configuration,
            IHttpClientFactory clientFactory
            )
        {
            _clientFactory = clientFactory;
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, ".NET apicaller");
        }

        public async Task<string> Helo()
        {
            return await HttpClient.GetStringAsync(this.ResouceUri("helo"));
        }

        public async Task<ActionResult<string>> Authenticate(string phonenumber)
        {
            var uri = ResouceUri("authenticatePath");
            var content = new StringContent(phonenumber);
            await _httpClient.PostAsync(uri, content);
            return "OK";
        }

        public async Task<ActionResult<string>> Verify(string accesscode)
        {
            var uri = ResouceUri("verifyPath");
            var content = new StringContent(accesscode);
            await _httpClient.PostAsync(uri, content);
            return "OK";
        }

        internal Uri ResouceUri(string command)
        {
            Uri retval;
            var builder = new UriBuilder(ServiceHost);
            builder.Path = _configuration.GetValue<string>("authenticatePath");
            Uri.TryCreate(builder.Uri, command, out retval);
            return retval;
        }
    }
}