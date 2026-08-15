using System.Globalization;
using System.IO;
using System.Linq;

namespace UnityFigmaMCP.Editor
{
    public static class FigmaAssetPathHelper
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars().Concat(new[] { ' ', '/' }).Distinct().ToArray();

        public static string SanitizeName(string name) => new(name.Where(c => !InvalidChars.Contains(c)).ToArray());

        public static string BuildAssetPath(string folder, string name, string extension)
        {
            var safeName = SanitizeName(name);
            var normalizedFolder = folder.TrimEnd('/') + "/";
            
            return $"{normalizedFolder}{safeName}.{extension}";
        }

        public static string ExtractVariantPrefabName(string originPrefabName, string variantName)
        {
            var sanitized = SanitizeName(variantName);
            var properties = sanitized.Split(',');
            
            foreach (var property in properties)
            {
                if (property.StartsWith("Prefab", true, CultureInfo.InvariantCulture) && TryExtractPropertyValue(property, out var prefabName))
                    return prefabName;
            }
            
            if (properties.Length > 0 && TryExtractPropertyValue(properties[0], out var firstPropertyValue))
                return $"{originPrefabName}_{firstPropertyValue}";

            return sanitized;
        }

        private static bool TryExtractPropertyValue(string property, out string value)
        {
            if (property.Contains('='))
            {
                var propertyValue = property.Split('=')[1];

                if (!string.IsNullOrEmpty(propertyValue))
                {
                    value = propertyValue;
                    return true;
                }
            }
            
            value = null;
            return false;
        }
    }
}
