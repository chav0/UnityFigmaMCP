using System.ComponentModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using UnityFigmaMCP.Common;
using UnityFigmaMCP.Server.Figma;
using UnityFigmaMCP.Server.Unity;

namespace UnityFigmaMCP.Server
{
    [McpServerToolType]
    public sealed class StatusMcpServerTools
    {
        private readonly IFigmaAuthService _figmaAuth;
        private readonly IUnityConnectionRegistry _unityConnections;

        public StatusMcpServerTools(IFigmaAuthService figmaAuth, IUnityConnectionRegistry unityConnections)
        {
            _figmaAuth = figmaAuth;
            _unityConnections = unityConnections;
        }

        [McpServerTool(Name = "status")]
        [Description("Check whether the server is ready to work: protocol version, whether the Figma access token is configured and actually valid, and whether a Unity Editor is connected to the SignalR hub. Call this before starting a Figma-to-Unity workflow, and first whenever a Figma or Unity tool fails — it tells you which half is broken.")]
        public async Task<string> GetStatus(CancellationToken ct = default)
        {
            var figma = await _figmaAuth.GetStatusAsync(ct).ConfigureAwait(false);
            var editorConnected = _unityConnections.Current != null;
            var figmaReady = figma.TokenValid == true;

            return JsonSerializer.Serialize(new
            {
                ready = figmaReady && editorConnected,
                protocolVersion = ProtocolVersion.Current,
                figma = new
                {
                    tokenConfigured = figma.TokenConfigured,
                    tokenValid = figma.TokenValid,
                    user = figma.UserHandle,
                    email = figma.UserEmail,
                    error = figma.Error
                },
                unity = new
                {
                    editorConnected,
                    hubUrl = SignalRDefaults.HubUrl
                },
                hint = Hint(figmaReady, editorConnected)
            });
        }

        private static string? Hint(bool figmaReady, bool editorConnected)
        {
            if (figmaReady && editorConnected)
                return null;

            if (!figmaReady && !editorConnected)
                return "Set FIGMA_ACCESS_TOKEN in the env block of your MCP client config, and open Tools > Unity Figma MCP in the Unity Editor and press Connect.";

            return figmaReady
                ? "Open Tools > Unity Figma MCP in the Unity Editor and press Connect. Figma tools work without it; Unity tools do not."
                : "Set FIGMA_ACCESS_TOKEN in the env block of your MCP client config. Unity tools work without it; Figma tools do not.";
        }
    }
}
