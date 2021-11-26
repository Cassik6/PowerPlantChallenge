using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PowerPlantChallenge.Services;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Middleware
{
    public class WebsocketMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketService _webSocketService;

        public WebsocketMiddleWare(RequestDelegate next, IWebSocketService webSocketService)
        {
            _next = next;
            this._webSocketService = webSocketService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    _webSocketService.AddWebSocket(webSocket);
                    await KeepOpen(webSocket);
                }
            }
            else
            {
                await _next(context);
            }
        }


        private static async Task KeepOpen(WebSocket webSocket)
        {            
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(new byte[1024 * 4]), CancellationToken.None);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
