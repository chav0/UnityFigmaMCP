using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityFigmaMCP.Common;
using UnityFigmaMCP.Editor.Exporters;

namespace UnityFigmaMCP.Editor
{
    internal sealed class BuildPrefabCommandHandler : ICommandHandler<BuildPrefabCommand, BuildPrefabCommandResult>
    {
        public BuildPrefabCommandResult Handle(ICommandContext context, BuildPrefabCommand command)
        {
            var file = FigmaFileLoader.Load(command.NodeJsonPath);
            if (file == null)
                throw new Exception($"File {command.NodeJsonPath} not found");

            var settings = context.LayoutSettings;
            var profile = settings.GetPipeline(command.PipelineId);
            if (profile == null)
                throw new Exception($"Pipeline {command.PipelineId} not found");

            var savePath = !string.IsNullOrEmpty(command.SavePath)
                ? command.SavePath
                : settings.PrefabFolderPath;

            if (!AssetDatabase.IsValidFolder(savePath))
                AssetDatabase.CreateFolder(
                    Path.GetDirectoryName(savePath),
                    Path.GetFileName(savePath));

            var exporter = new PrefabExporter(settings, file, profile);
            var prefab = exporter.Export(command.PrefabName, savePath);
            if (prefab == null)
                return null;

            var assetPath = AssetDatabase.GetAssetPath(prefab);

            var result = new BuildPrefabCommandResult
            {
                Root = UnityObjectConverter.Convert(prefab, context.Mappers.All, assetPath)
            };

            var component = settings.ComponentMap.FindComponent(null, prefab.name);
            if (component != null && component.variants.Count > 0)
            {
                result.Variants = component.variants
                    .Where(variant => variant.prefab != null)
                    .Select(variant => new AssetInfo
                    {
                        Name = variant.name,
                        Path = AssetDatabase.GetAssetPath(variant.prefab),
                        FigmaKey = variant.id
                    })
                    .ToArray();
            }

            return result;
        }
    }
}
