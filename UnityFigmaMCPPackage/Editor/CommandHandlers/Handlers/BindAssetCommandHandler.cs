using System;
using UnityEditor;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class BindAssetCommandHandler : ICommandHandler<BindAssetCommand, BindAssetCommandResult>
    {
        public BindAssetCommandResult Handle(ICommandContext context, BindAssetCommand command)
        {
            var results = new AssetInfo[command.Assets.Length];

            for (var i = 0; i < command.Assets.Length; i++)
            {
                var entry = command.Assets[i];

                switch (command.Kind)
                {
                    case AssetKinds.Prefab:
                        results[i] = BindPrefab(entry);
                        break;

                    case AssetKinds.Sprite:
                        results[i] = BindSprite(entry);
                        break;

                    case AssetKinds.Variant:
                        results[i] = BindVariant(entry);
                        break;

                    default:
                        throw new Exception(
                            $"Unknown asset kind \"{command.Kind}\". Use \"{AssetKinds.Prefab}\", \"{AssetKinds.Sprite}\" or \"{AssetKinds.Variant}\".");
                }
            }

            return new BindAssetCommandResult { Kind = command.Kind, Assets = results };
        }

        private static AssetInfo BindPrefab(BindEntry entry)
        {
            if (string.IsNullOrEmpty(entry.FigmaKey))
                throw new Exception($"Could not resolve a component key for '{entry.AssetPath}'. Ensure the node was fetched via figma_get_node first.");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath);
            if (prefab == null)
                throw new Exception($"Prefab not found at '{entry.AssetPath}'");

            FigmaComponentMap.GetOrCreate().AddComponent(entry.FigmaKey, prefab.name, prefab);

            return new AssetInfo { Name = prefab.name, Path = entry.AssetPath, FigmaKey = entry.FigmaKey };
        }

        private static AssetInfo BindVariant(BindEntry entry)
        {
            if (string.IsNullOrEmpty(entry.FigmaKey))
                throw new Exception($"Could not resolve a variant key for '{entry.AssetPath}'. Ensure the node was fetched via figma_get_node first.");

            if (string.IsNullOrEmpty(entry.ParentKey))
                throw new Exception($"'{entry.AssetPath}' is not part of a Figma component set. Bind it as \"{AssetKinds.Prefab}\" instead.");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath);
            if (prefab == null)
                throw new Exception($"Prefab not found at '{entry.AssetPath}'");

            var variantName = !string.IsNullOrEmpty(entry.FigmaName) ? entry.FigmaName : prefab.name;

            FigmaComponentMap.GetOrCreate()
                .AddVariant(entry.ParentKey, entry.ParentName, entry.FigmaKey, variantName, prefab);

            return new AssetInfo { Name = variantName, Path = entry.AssetPath, FigmaKey = entry.FigmaKey };
        }

        private static AssetInfo BindSprite(BindEntry entry)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(entry.AssetPath);
            if (sprite == null)
                throw new Exception($"Sprite not found at '{entry.AssetPath}'");

            FigmaSpriteMap.GetOrCreate().Add(sprite.name, sprite, entry.FigmaKey, entry.FigmaName);

            return new AssetInfo { Name = sprite.name, Path = entry.AssetPath, FigmaKey = entry.FigmaKey };
        }
    }
}
