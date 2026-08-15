using System.IO;
using UnityEditor;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class SavePrefabCommandHandler : ICommandHandler<SavePrefabCommand, SavePrefabCommandResult>
    {
        public SavePrefabCommandResult Handle(ICommandContext context, SavePrefabCommand command)
        {
            using var scope = PrefabEditScope.Create(command.PrefabPath, command.ObjectPath);

            var directory = Path.GetDirectoryName(command.AssetPath);
            
            if (!string.IsNullOrEmpty(directory) && !AssetDatabase.IsValidFolder(directory))
                AssetDatabase.CreateFolder(
                    Path.GetDirectoryName(directory),
                    Path.GetFileName(directory));

            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                scope.Target, command.AssetPath, InteractionMode.AutomatedAction);

            if (!string.IsNullOrEmpty(command.ComponentKey))
                FigmaComponentMap.GetOrCreate().AddComponent(command.ComponentKey, prefab.name, prefab);

            scope.Save();

            return new SavePrefabCommandResult
            {
                AssetPath = command.AssetPath,
                Root = UnityObjectConverter.Convert(prefab, context.Mappers.All, command.AssetPath)
            };
        }
    }
}
