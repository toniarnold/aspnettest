using System.Net.WebSockets;
using System.Threading.Tasks;

namespace asplib.Services
{
    /// <summary>
    /// Common client for WebSockets.ClientWebSocket and
    /// TestHost.WebSocketClient These objects have no common ancestors.
    /// </summary>
    public interface IWebSocketClient
    {
        public Task<WebSocket> ConnectAsync();
    }
}