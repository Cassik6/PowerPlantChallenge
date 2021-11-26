using PowerPlantChallenge.Models;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Services
{
    public interface IWebSocketService
    {
        void AddWebSocket(WebSocket webSocket);
        Task SendPowerProduction(List<PowerProduction> powerProductions);
    }
}