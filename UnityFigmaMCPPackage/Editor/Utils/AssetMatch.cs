using UnityEditor;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    public sealed class AssetMatch
    {
        public AssetMatchKind Kind { get; }
        public AssetMatchSource Source { get; }
        public GameObject Prefab { get; }
        public Sprite Sprite { get; }

        public Object Asset => Kind == AssetMatchKind.Sprite ? Sprite : Prefab;
        public string Path => AssetDatabase.GetAssetPath(Asset);

        private AssetMatch(AssetMatchKind kind, AssetMatchSource source, GameObject prefab, Sprite sprite)
        {
            Kind = kind;
            Source = source;
            Prefab = prefab;
            Sprite = sprite;
        }

        public static AssetMatch ForPrefab(FigmaComponentMatch match, AssetMatchSource source)
            => new(match.Variant != null ? AssetMatchKind.Variant : AssetMatchKind.Prefab, source, match.Prefab, null);

        public static AssetMatch ForPrefab(GameObject prefab, AssetMatchSource source)
            => new(AssetMatchKind.Prefab, source, prefab, null);

        public static AssetMatch ForSprite(Sprite sprite, AssetMatchSource source)
            => new(AssetMatchKind.Sprite, source, null, sprite);
    }
    
    public enum AssetMatchKind
    {
        Prefab,
        Variant,
        Sprite
    }

    public enum AssetMatchSource
    {
        Key,
        Name,
        ImageRef
    }
}