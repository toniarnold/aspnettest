using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace asplib.Services
{
    /// <summary>
    /// IWebSocketClient client connecting to a TestServer
    /// </summary>
    public class TestServerWebSocketClient : IWebSocketClient
    {
        private readonly WebSocketClient _client;
        private readonly Uri _uri;

        public TestServerWebSocketClient(TestServer testServer, string path)
        {
            _client = testServer.CreateWebSocketClient();
            var builder = new UriBuilder(testServer.BaseAddress) { Scheme = "ws", Path = path };
            _uri = builder.Uri;
        }

        public Task<WebSocket> ConnectAsync()
        {
            return (Task<WebSocket>)_client.ConnectAsync(_uri, CancellationToken.None);
        }
    }
}