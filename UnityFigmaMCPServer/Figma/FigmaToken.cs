using System;

namespace UnityFigmaMCP.Server.Figma
{
    internal static class FigmaToken
    {
        public const string EnvVar = "FIGMA_ACCESS_TOKEN";

        public static string Value => Environment.GetEnvironmentVariable(EnvVar);

        public static bool IsSet => !string.IsNullOrWhiteSpace(Value);
    }
}
