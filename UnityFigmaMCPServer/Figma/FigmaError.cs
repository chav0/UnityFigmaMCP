using System;
using System.Net.Http;

namespace UnityFigmaMCP.Server.Figma
{
    internal static class FigmaError
    {
        public static string ToDescription(this Exception ex)
        {
            if (!FigmaToken.IsSet)
                return "FIGMA_ACCESS_TOKEN is not set. Add it to the env block in your MCP client config.";

            return ex is HttpRequestException http
                ? $"Figma API returned HTTP {(int)(http.StatusCode ?? 0)}: {http.Message}"
                : ex.Message;
        }
    }
}
