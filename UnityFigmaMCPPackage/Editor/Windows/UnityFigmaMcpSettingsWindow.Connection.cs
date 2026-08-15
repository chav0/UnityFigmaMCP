using System;
using System.Threading;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    public sealed partial class UnityFigmaMcpSettingsWindow
    {
        private bool _busy;

        private void ToggleConnection()
        {
            if (_busy)
                return;

            if (_connection != null && _connection.IsConnected)
            {
                _busy = true;
                try
                {
                    _connection.DisconnectBlocking();
                }
                finally
                {
                    OnDisconnected();
                }

                return;
            }

            RunConnectAsync();
        }

        private async void RunConnectAsync()
        {
            if (_busy || _connectButton == null)
                return;

            _busy = true;
            _connectButton.SetEnabled(false);

            CancelConnect();

            _connectCancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = _connectCancellationTokenSource.Token;
            var url = SignalRDefaults.HubUrl;

            try
            {
                var connection = new UnityMcpConnection(AppendLog);

                await connection.ConnectAsync(url, cancellationToken);

                _connection = connection;
                OnConnected(url);
            }
            catch (OperationCanceledException)
            {
                OnConnectionCancelled();
            }
            catch (Exception ex)
            {
                OnConnectionFailed(ex);
            }
        }

        private void OnConnected(string url)
        {
            _busy = false;
            
            AppendLog("Connected to server (" + url + ").");
            SetConnectedStatus();
        }

        private void OnConnectionCancelled()
        {
            _busy = false;
            
            AppendLog("Connection cancelled.");
            SetDisconnectedStatus();
        }

        private void OnConnectionFailed(Exception ex)
        {
            _connection = null;
            _busy = false;
            
            AppendLog("Failed to connect: " + ex.Message);
            SetConnectionErrorStatus();
        }

        private void OnDisconnected()
        {
            _connection = null;
            _busy = false;
                    
            AppendLog("Disconnected from server.");
            SetDisconnectedStatus();
        }

        private async void OnPingClicked()
        {
            if (_connection == null || !_connection.IsConnected)
                return;

            try
            {
                var response = await _connection.PingAsync("unity-editor");
                AppendLog("Ping: server responded — " + response);
            }
            catch (Exception ex)
            {
                AppendLog("Ping: no response — " + ex.Message);
            }
        }

        private void CancelConnect()
        {
            if (_connectCancellationTokenSource == null)
                return;

            _connectCancellationTokenSource.Cancel();
            _connectCancellationTokenSource.Dispose();
            _connectCancellationTokenSource = null;
        }
    }
}
