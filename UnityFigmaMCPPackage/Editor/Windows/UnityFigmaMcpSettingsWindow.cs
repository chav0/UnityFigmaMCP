using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    public sealed partial class UnityFigmaMcpSettingsWindow : EditorWindow
    {
        private const string PackagePath = "Packages/com.hugglebit.unity.figma.mcp/Editor/Windows/";
        private const long HealthCheckIntervalMs = 3000;

        private UnityMcpConnection _connection;
        private CancellationTokenSource _connectCancellationTokenSource;
        private Button _connectButton;
        private Button _pingButton;
        private Label _statusLabel;
        private ScrollView _logView;

        [MenuItem("Tools/Unity Figma MCP")]
        public static void Open()
        {
            var window = GetWindow<UnityFigmaMcpSettingsWindow>();
            window.titleContent = new GUIContent("Unity Figma MCP");
            window.minSize = new Vector2(440, 360);
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();

            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PackagePath + "UnityFigmaMcpSettingsWindow.uxml");

            if (uxml == null)
            {
                rootVisualElement.Add(new HelpBox("UXML not found at: " + PackagePath, HelpBoxMessageType.Error));
                return;
            }

            uxml.CloneTree(rootVisualElement);

            var hubUrlLabel = rootVisualElement.Q<Label>("hub-url-display");
            if (hubUrlLabel != null)
                hubUrlLabel.text = SignalRDefaults.HubUrl;

            _connectButton = rootVisualElement.Q<Button>("btn-connect");
            _pingButton = rootVisualElement.Q<Button>("btn-ping");
            var clearButton = rootVisualElement.Q<Button>("btn-clear-log");
            _statusLabel = rootVisualElement.Q<Label>("status-label");
            _logView = rootVisualElement.Q<ScrollView>("log-view");

            if (_connectButton != null)
                _connectButton.clicked += ToggleConnection;

            if (_pingButton != null)
            {
                _pingButton.clicked += OnPingClicked;
                _pingButton.SetEnabled(false);
            }

            if (clearButton != null)
            {
                clearButton.clicked += () =>
                {
                    _logView?.Clear();
                    
                    AppendLog("Log cleared.");
                };
            }

            ScheduleHealthCheck();

            AppendLog("Ready. Press \"Connect\" to connect to the MCP server.");
        }

        private void AppendLog(string line)
        {
            if (_logView == null)
                return;

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var row = new Label("[" + timestamp + "] " + line);
            
            row.AddToClassList("mcp-log-line");
            
            _logView.Add(row);
            _logView.schedule.Execute(() =>
            {
                _logView.verticalScroller.value = _logView.verticalScroller.highValue;
            }).ExecuteLater(0);
        }
        
        private void OnDisable()
        {
            _healthCheck?.Pause();

            if (_connectButton != null)
                _connectButton.clicked -= ToggleConnection;
            
            if (_pingButton != null)
                _pingButton.clicked -= OnPingClicked;

            CancelConnect();

            if (_connection != null)
            {
                _connection.DisconnectBlocking();
                _connection = null;
            }
        }
    }
}
