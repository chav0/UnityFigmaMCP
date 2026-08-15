using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine.UIElements;

namespace UnityFigmaMCP.Editor
{
    public sealed partial class UnityFigmaMcpSettingsWindow
    {
        private IVisualElementScheduledItem _healthCheck;
        private HubConnectionState _lastKnownState = HubConnectionState.Disconnected;

        private void ScheduleHealthCheck()
        {
            if (_healthCheck != null)
            {
                _healthCheck.Resume();
            }
            else
            {
                _healthCheck = rootVisualElement.schedule
                    .Execute(SyncConnectionState)
                    .Every(HealthCheckIntervalMs);
            }
        }

        private void SyncConnectionState()
        {
            if (_busy)
                return;

            var currentState = _connection?.State ?? HubConnectionState.Disconnected;
            if (currentState == _lastKnownState)
                return;

            switch (currentState)
            {
                case HubConnectionState.Connected:
                    SetConnectedStatus();
                    break;

                case HubConnectionState.Reconnecting:
                    SetReconnectingStatus();
                    break;

                case HubConnectionState.Disconnected:
                    AppendLog("Connection lost. Make sure the MCP server is running.");
                    SetDisconnectedStatus();
                    break;
            }
        }

        private void SetConnectedStatus()
        {
            _lastKnownState = HubConnectionState.Connected;
            
            SetStatus("Connected.");

            if (_connectButton != null)
            {
                _connectButton.text = "Disconnect";
                _connectButton.SetEnabled(true);
            }

            _pingButton?.SetEnabled(true);
        }

        private void SetReconnectingStatus()
        {
            _lastKnownState = HubConnectionState.Reconnecting;
            
            SetStatus("Reconnecting…");

            _pingButton?.SetEnabled(false);
        }

        private void SetDisconnectedStatus()
        {
            _lastKnownState = HubConnectionState.Disconnected;
            
            SetStatus("Disconnected.");

            if (_connectButton != null)
            {
                _connectButton.text = "Connect";
                _connectButton.SetEnabled(true);
            }

            _pingButton?.SetEnabled(false);
        }

        private void SetConnectionErrorStatus()
        {
            _lastKnownState = HubConnectionState.Disconnected;
            
            SetStatus("Connection error.");

            _connectButton?.SetEnabled(true);
            _pingButton?.SetEnabled(false);
        }

        private void SetStatus(string text)
        {
            if (_statusLabel != null)
                _statusLabel.text = "Status: " + text;
        }
    }
}
