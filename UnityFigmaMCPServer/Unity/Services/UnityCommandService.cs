using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Server.Unity
{
    internal sealed class UnityCommandService : IUnityCommandService
    {
        private static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(120);

        private readonly IHubContext<UnityMcpHub> _hubContext;
        private readonly IUnityConnectionRegistry _connections;

        public UnityCommandService(IHubContext<UnityMcpHub> hubContext, IUnityConnectionRegistry connections)
        {
            _hubContext = hubContext;
            _connections = connections;
        }

        public async Task<string> SendCommandAsync<TCommand>(TCommand command, CancellationToken ct)
            where TCommand : ICommand
        {
            var connectionId = _connections.Current
                ?? throw new InvalidOperationException(
                    "No Unity Editor is connected. Open the project and press \"Connect\" in Tools > Unity Figma MCP.");

            var method = command.GetType().Name;
            var payloadJson = JsonConvert.SerializeObject(command);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(CommandTimeout);

            try
            {
                return await _hubContext.Clients
                    .Client(connectionId)
                    .InvokeAsync<string>(method, payloadJson, timeoutCts.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                throw new TimeoutException(
                    $"Unity Editor did not respond to {method} within {CommandTimeout.TotalSeconds:0}s. " +
                    "Is the Editor still connected and not blocked by a modal dialog?");
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException(
                    $"Unity Editor disconnected while handling {method}: {ex.Message}", ex);
            }
        }
    }
}
