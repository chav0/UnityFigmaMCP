using System.Globalization;
using System.IO;

namespace UnityFigmaMCP.Server.Figma
{
    internal static class FigmaPathHelper
    {
        private const string TempDirName = "unity-figma-mcp";

        internal static string GetFileDir(string fileKey)
        {
            return Path.Combine(Path.GetTempPath(), TempDirName, fileKey);
        }

        internal static string GetNodePath(string fileKey, string nodeId)
        {
            var dir = GetFileDir(fileKey);
            Directory.CreateDirectory(dir);

            var safeNodeId = nodeId.Replace(':', '-');
            return Path.Combine(dir, safeNodeId + ".json");
        }

        internal static string GetImagePath(string fileKey, string nodeId, float scale)
        {
            var dir = GetFileDir(fileKey);
            Directory.CreateDirectory(dir);

            var safeNodeId = nodeId.Replace(':', '-');
            var scaleStr = scale.ToString("0.##", CultureInfo.InvariantCulture);
            return Path.Combine(dir, $"{safeNodeId}@{scaleStr}x.png");
        }
    }
}
