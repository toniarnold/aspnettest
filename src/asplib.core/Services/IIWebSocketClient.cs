using System.Net.WebSockets;
using System.Threading.Tasks;

namespace asplib.Services
{
    /// <summary>
    /// Common client for WebSockets.ClientWebSocket and
    /// TestHost.WebSocketClient These objects have no common ancestors,
    /// therefore use dynamic for the return value of ConnectAsync() such that
    /// both can SendAsync()
    /// </summary>
    public interface IWebSocketClient
    {
        public Task<WebSocket> ConnectAsync();
    }
}