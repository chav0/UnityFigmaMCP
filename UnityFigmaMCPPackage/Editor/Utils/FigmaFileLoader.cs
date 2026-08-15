using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal static class FigmaFileLoader
    {
        internal static FigmaFile Load(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<FigmaFile>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to read FigmaFile at {path}: {e.Message}");
                return null;
            }
        }
    }
}
