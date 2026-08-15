using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class ListAssetsCommandHandler : ICommandHandler<ListAssetsCommand, ListAssetsCommandResult>
    {
        public ListAssetsCommandResult Handle(ICommandContext context, ListAssetsCommand command)
        {
            var settings = context.LayoutSettings;

            string folder;
            string filter;
            Func<string, AssetInfo> describe;

            switch (command.Kind)
            {
                case AssetKinds.Prefab:
                    folder = settings.PrefabFolderPath;
                    filter = "t:Prefab";
                    describe = DescribePrefab;
                    break;

                case AssetKinds.Sprite:
                    folder = settings.SpritesFolderPath;
                    filter = "t:Sprite";
                    describe = DescribeSprite;
                    break;

                default:
                    throw new Exception(
                        $"Unknown asset kind \"{command.Kind}\". Use \"{AssetKinds.Prefab}\" or \"{AssetKinds.Sprite}\".");
            }

            var searchFolder = !string.IsNullOrEmpty(folder) ? folder : "Assets";
            var guids = AssetDatabase.FindAssets(filter, new[] { searchFolder });
            var assets = new List<AssetInfo>(guids.Length);

            foreach (var guid in guids)
            {
                var info = describe(AssetDatabase.GUIDToAssetPath(guid));
                if (info != null)
                    assets.Add(info);
            }

            return new ListAssetsCommandResult
            {
                Kind = command.Kind,
                Folder = searchFolder,
                Assets = assets.ToArray()
            };
        }

        private static AssetInfo DescribePrefab(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                return null;

            var component = FigmaComponentMap.GetOrCreate().FindComponent(null, prefab.name);

            var info = new AssetInfo
            {
                Name = prefab.name,
                Path = path,
                FigmaKey = component?.id
            };

            if (component != null && component.variants.Count > 0)
            {
                info.Variants = component.variants
                    .Where(variant => variant.prefab != null)
                    .Select(variant => new AssetInfo
                    {
                        Name = variant.name,
                        Path = AssetDatabase.GetAssetPath(variant.prefab),
                        FigmaKey = variant.id
                    })
                    .ToArray();
            }

            return info;
        }

        private static AssetInfo DescribeSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
                return null;

            return new AssetInfo
            {
                Name = sprite.name,
                Path = path,
                FigmaKey = FigmaSpriteMap.GetOrCreate().FindId(sprite.name)
            };
        }
    }
}
