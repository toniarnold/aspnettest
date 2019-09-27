using apicaller.Services.Dto;
using apiservice.View;
using asplib.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;
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

        public string[] Cookies { get; set; }

        internal virtual Uri ServiceHost
        {
            get { return new Uri(_configuration.GetValue<string>("apiserviceHost")); }
        }

        internal HttpClient GetHttpClient()
        {
            var client = _clientFactory.CreateClient("ServiceClient");
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
                var request = new AuthenticateRequest()
                {
                    Phonenumber = phonenumber
                };
                var uri = ResouceUri("authenticate");
                var response = await client.PostAsync(uri, Json.Serialize(request));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                }
                Cookies = response.Headers.GetValues(SetCookie).ToArray();
                var result = Json.Deserialize<MessageResponseDto>(response.Content);
                return result.Message;
            }
        }

        public async Task<ActionResult<string>> Verify(string accesscode)
        {
            using (var client = GetHttpClient())
            {
                var request = new VerifyRequest()
                {
                    Accesscode = accesscode
                };
                var uri = ResouceUri("verify");
                var response = await client.PostAsync(uri, Json.Serialize(request));
                var result = Json.Deserialize<MessageResponseDto>(response.Content);
                return result.Message;
            }
        }

        internal Uri ResouceUri(string command)
        {
            Uri retval;
            var builder = new UriBuilder(ServiceHost);
            builder.Path = _configuration.GetValue<string>("accesscodePath").TrimEnd('/') + "/";
            Uri.TryCreate(builder.Uri, command, out retval);
            return retval;
        }

        internal void AddDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(Application.Json));
            client.DefaultRequestHeaders.Add(UserAgent, ".NET HttpClient");
            foreach (var cookie in Cookies ?? Enumerable.Empty<string>())
            {
                client.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookie);
            }
        }
    }
}