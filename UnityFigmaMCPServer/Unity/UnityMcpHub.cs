using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace UnityFigmaMCP.Server.Unity
{
    public sealed class UnityMcpHub : Hub
    {
        private readonly IUnityConnectionRegistry _connections;

        public UnityMcpHub(IUnityConnectionRegistry connections) => _connections = connections;

        public Task<string> Ping(string message) => Task.FromResult("ack:" + message);

        public override Task OnConnectedAsync()
        {
            _connections.Add(Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _connections.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
