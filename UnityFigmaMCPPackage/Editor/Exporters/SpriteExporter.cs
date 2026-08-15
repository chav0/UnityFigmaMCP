using System.IO;
using UnityEditor;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor.Exporters
{
    public class SpriteExporter
    {
        private readonly FigmaSpriteMap _spriteMap;

        public SpriteExporter(FigmaSpriteMap spriteMap)
        {
            _spriteMap = spriteMap;
        }

        public string Export(string folderPath, string spriteName, byte[] pngBytes, string id = null, string figmaName = null)
        {
            EnsureFolder(folderPath);
            var assetPath = WriteAndImport(folderPath, spriteName, pngBytes);
            ConfigureImporter(assetPath);
            RegisterSprite(spriteName, assetPath, id, figmaName);
            return assetPath;
        }

        public string[] ExportBatch(string folderPath, SpriteEntry[] entries)
        {
            EnsureFolder(folderPath);
            var assetPaths = new string[entries.Length];

            AssetDatabase.StartAssetEditing();
            try
            {
                for (var i = 0; i < entries.Length; i++)
                {
                    var pngBytes = File.ReadAllBytes(entries[i].SpritePath);
                    assetPaths[i] = WriteAndImport(folderPath, entries[i].SpriteName, pngBytes);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var assetPath in assetPaths)
                    ConfigureImporter(assetPath);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            for (var i = 0; i < entries.Length; i++)
                RegisterSprite(entries[i].SpriteName, assetPaths[i], entries[i].Id, entries[i].FigmaName);

            return assetPaths;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder(
                    Path.GetDirectoryName(folderPath),
                    Path.GetFileName(folderPath));
        }

        private static string WriteAndImport(string folderPath, string spriteName, byte[] pngBytes)
        {
            var assetPath = FigmaAssetPathHelper.BuildAssetPath(folderPath, spriteName, "png");
            File.WriteAllBytes(assetPath, pngBytes);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            return assetPath;
        }

        private static void ConfigureImporter(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }

        private void RegisterSprite(string spriteName, string assetPath, string id, string figmaName)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
                _spriteMap.Add(spriteName, sprite, id, figmaName);
        }
    }
}
