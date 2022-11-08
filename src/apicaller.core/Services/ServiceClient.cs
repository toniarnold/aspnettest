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
        internal IConfiguration _configuration = default!;
        internal IHttpClientFactory _clientFactory = default!;

        public string[] Cookies { get; set; } = Array.Empty<string>();

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
                var request = new AuthenticateRequest(phonenumber);
                var response = await client.PostAsync(ResouceUri("authenticate"), JsonContent.Serialize(request));
                if (response.StatusCode != HttpStatusCode.OK) return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
                this.Cookies = response.Headers.GetValues(SetCookie).ToArray();
                var result = JsonContent.Deserialize<MessageResponseDto>(response.Content) ??
                    throw new Exception("Null response"); ;
                return result.Message;
            }
        }

        public async Task<ActionResult<string>> Verify(string accesscode)
        {
            using (var client = GetHttpClient())
            {
                var request = new VerifyRequest(accesscode);
                var response = await client.PostAsync(ResouceUri("verify"), JsonContent.Serialize(request));
                if (response.StatusCode != HttpStatusCode.OK) return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
                var result = JsonContent.Deserialize<MessageResponseDto>(response.Content) ??
                    throw new Exception("Null response"); ;
                return result.Message;
            }
        }

        internal Uri ResouceUri(string command)
        {
            Uri retval;
            var builder = new UriBuilder(ServiceHost);
            builder.Path = _configuration.GetValue<string>("accesscodePath").TrimEnd('/') + "/";
            if (!Uri.TryCreate(builder.Uri, command, out retval!))
            {
                throw new Exception($"Could not create Uri for path {builder.Path} and command {command}");
            }
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