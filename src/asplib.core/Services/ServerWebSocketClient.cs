using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace asplib.Services
{
    /// <summary>
    /// IWebSocketClient client connecting to a real TCP/IP server
    /// </summary>
    public class ServerWebSocketClient : IWebSocketClient, IDisposable
    {
        private readonly ClientWebSocket _client;
        private readonly Uri _uri;

        public ServerWebSocketClient(Uri uri)
        {
            _client = new ClientWebSocket();
            _uri = uri;
        }

        public async Task<WebSocket> ConnectAsync()
        {
            await _client.ConnectAsync(_uri, CancellationToken.None);
            return _client;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }
                disposedValue = true;
            }
        }
    }
}