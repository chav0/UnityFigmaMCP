using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class UnityMcpConnection : IDisposable
    {
        private readonly Action<string> _log;

        private HubConnection _hubConnection;
        private UnityCommandRouter _router;

        public UnityMcpConnection(Action<string> log) => _log = log;

        public bool IsConnected => State == HubConnectionState.Connected;

        public HubConnectionState State => _hubConnection?.State ?? HubConnectionState.Disconnected;

        public async Task ConnectAsync(string hubUrl, CancellationToken cancellationToken = default)
        {
            await DisconnectAsync().ConfigureAwait(false);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.Reconnecting += _ =>
            {
                EditorMainThreadQueue.Enqueue(() => _log("Reconnecting to server…"));
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += _ =>
            {
                EditorMainThreadQueue.Enqueue(() => _log("Connection restored."));
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                var msg = error != null
                    ? "Connection lost: " + error.Message
                    : "Connection closed.";

                EditorMainThreadQueue.Enqueue(() => _log(msg));
                return Task.CompletedTask;
            };

            _router = new UnityCommandRouter(_hubConnection, _log);

            await _hubConnection.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection == null)
                return;

            _router?.Dispose();
            _router = null;

            try
            {
                await _hubConnection.StopAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                EditorMainThreadQueue.Enqueue(() => _log("Disconnect error: " + ex.Message));
            }

            try
            {
                await _hubConnection.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                EditorMainThreadQueue.Enqueue(() => _log("Cleanup error: " + ex.Message));
            }

            _hubConnection = null;
        }

        public void DisconnectBlocking()
        {
            if (_hubConnection == null)
                return;

            try
            {
                Task.Run(DisconnectAsync).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _log("Disconnect error: " + ex.Message);
            }
        }

        public Task<string> PingAsync(string message, CancellationToken cancellationToken = default)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
                throw new InvalidOperationException("Not connected to server.");

            return _hubConnection.InvokeAsync<string>(SignalRDefaults.PingMethod, message, cancellationToken);
        }

        public void Dispose()
        {
            DisconnectBlocking();
        }
    }
}
