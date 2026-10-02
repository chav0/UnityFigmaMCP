using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    public sealed class FigmaComponentMatch
    {
        public FigmaComponent Component { get; }
        public FigmaComponentVariant Variant { get; }

        public GameObject Prefab => Variant?.prefab != null ? Variant.prefab : Component.prefab;

        public FigmaComponentMatch(FigmaComponent component, FigmaComponentVariant variant)
        {
            Component = component;
            Variant = variant;
        }
    }
}