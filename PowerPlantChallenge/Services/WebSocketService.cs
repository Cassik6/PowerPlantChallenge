using PowerPlantChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Services
{
    public class WebSocketService : IWebSocketService
    {
        private static List<WebSocket> webSockets;

        public WebSocketService()
        {
            if (webSockets == null)
            {
                webSockets = new List<WebSocket>();
            }
        }

        public void AddWebSocket(WebSocket webSocket)
        {
            webSockets.Add(webSocket);
        }

        public async Task SendPowerProduction(List<PowerProduction> powerProductions)
        {
            var jsonString = JsonSerializer.Serialize(powerProductions);
            var bytes = Encoding.UTF8.GetBytes(jsonString);

            foreach (var webSocket in webSockets)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    Console.WriteLine(jsonString);
                    await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
