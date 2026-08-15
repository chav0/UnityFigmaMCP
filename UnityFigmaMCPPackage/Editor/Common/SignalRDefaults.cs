namespace UnityFigmaMCP.Common
{
    public static class SignalRDefaults
    {
        public const string DefaultHost = "127.0.0.1";
        public const int HttpPort = 52802;
        public const string HubPath = "/hubs/unity-mcp";
        public static string BaseUrl => $"http://{DefaultHost}:{HttpPort}";
        public static string HubUrl => BaseUrl + HubPath;

        public const string PingMethod = "Ping";
    }
}
