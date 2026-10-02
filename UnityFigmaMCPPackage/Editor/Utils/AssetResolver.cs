using UnityEditor;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    public sealed class AssetResolver
    {
        private readonly FigmaComponentMap _componentMap;
        private readonly FigmaSpriteMap _spriteMap;
        private readonly FigmaFile _file;
        private readonly string _prefabFolder;

        public FigmaSpriteMap SpriteMap => _spriteMap;

        public AssetResolver(FigmaComponentMap componentMap, FigmaSpriteMap spriteMap, FigmaFile file, string prefabFolder)
        {
            _componentMap = componentMap;
            _spriteMap = spriteMap;
            _file = file;
            _prefabFolder = string.IsNullOrEmpty(prefabFolder) ? "Assets" : prefabFolder.TrimEnd('/');
        }

        public string GetComponentKey(FigmaObject instance)
            => _file?.GetComponentKey(instance.componentId) ?? instance.componentId;

        public AssetMatch ResolvePrefab(FigmaObject instance)
        {
            if (instance == null || instance.type != FigmaObjectType.INSTANCE)
                return null;

            var key = GetComponentKey(instance);
            var prefabName = FigmaAssetPathHelper.SanitizeName(instance.name);

            var mapped = _componentMap.FindMatch(key);
            if (mapped != null)
                return AssetMatch.ForPrefab(mapped, AssetMatchSource.Key);

            var spriteByKey = _spriteMap.Find(key);
            if (spriteByKey != null)
                return AssetMatch.ForSprite(spriteByKey, AssetMatchSource.Key);

            mapped = _componentMap.FindMatch(null, prefabName);
            if (mapped != null)
                return AssetMatch.ForPrefab(mapped, AssetMatchSource.Name);

            var prefab = FindPrefabInFolder(prefabName);
            if (prefab != null)
                return AssetMatch.ForPrefab(prefab, AssetMatchSource.Name);

            var spriteByName = _spriteMap.Find(instance.name);
            
            return spriteByName != null ? AssetMatch.ForSprite(spriteByName, AssetMatchSource.Name) : null;
        }

        public AssetMatch ResolveSprite(FigmaObject node)
        {
            if (node == null)
                return null;

            var mapped = _spriteMap.Find(node.name);
            if (mapped != null)
                return AssetMatch.ForSprite(mapped, AssetMatchSource.Name);

            if (node.fills != null)
            {
                foreach (var fill in node.fills)
                {
                    if (fill.type != "IMAGE")
                        continue;

                    var sprite = _spriteMap.Find(fill.imageRef) ?? FindSpriteInProject(fill.imageRef);
                    if (sprite != null)
                        return AssetMatch.ForSprite(sprite, AssetMatchSource.ImageRef);
                }
            }

            var found = FindSpriteInProject(node.name);
            
            return found != null ? AssetMatch.ForSprite(found, AssetMatchSource.Name) : null;
        }

        private GameObject FindPrefabInFolder(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
                return null;

            foreach (var guid in AssetDatabase.FindAssets($"t:Prefab {prefabName}", new[] { _prefabFolder }))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (prefab != null && prefab.name == prefabName)
                    return prefab;
            }

            return null;
        }

        private static Sprite FindSpriteInProject(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
                return null;

            foreach (var guid in AssetDatabase.FindAssets($"t:Sprite {spriteName}", new[] { "Assets" }))
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
                if (sprite != null && sprite.name == spriteName)
                    return sprite;
            }

            return null;
        }
    }
}
