using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ChatServiceBackend;
public class WebSocketMiddleware
{
    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
    private readonly RequestDelegate _next;

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string socketId = Guid.NewGuid().ToString();
            _sockets.TryAdd(socketId, webSocket);

            await ReceiveMessage(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text && result.EndOfMessage)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await BroadcastMessage(socketId, message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    _sockets.TryRemove(socketId, out WebSocket dummy);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            });
        }
        else
        {
            await _next(context);
        }
    }

    private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            handleMessage(result, buffer);
        }
    }

    private async Task BroadcastMessage(string senderSocketId, string message)
    {
        foreach (var socket in _sockets)
        {
            if (socket.Key != senderSocketId) // Omitir al remitente
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    await socket.Value.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
