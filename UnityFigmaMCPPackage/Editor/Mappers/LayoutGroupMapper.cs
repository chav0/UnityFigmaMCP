using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityFigmaMCP.Editor
{
    internal abstract class LayoutGroupMapper<TUnity, TDto> : ComponentMapper<TUnity, TDto>
        where TUnity : LayoutGroup
    {
        public override void Apply(GameObject gameObject, TDto dto)
        {
            RemoveAllLayouts(gameObject);
            base.Apply(gameObject, dto);
        }

        public override void Remove(GameObject gameObject) => RemoveAllLayouts(gameObject);

        protected static void ApplyAlignment(LayoutGroup layout, string alignment)
        {
            if (!string.IsNullOrEmpty(alignment) && Enum.TryParse<TextAnchor>(alignment, true, out var anchor))
                layout.childAlignment = anchor;
        }

        protected static void ApplyPadding(LayoutGroup layoutGroup, float[] padding)
        {
            if (padding == null || padding.Length < 4)
                return;

            layoutGroup.padding = new RectOffset(
                (int)padding[0],
                (int)padding[1],
                (int)padding[2],
                (int)padding[3]);
        }

        private static void RemoveAllLayouts(GameObject gameObject)
        {
            foreach (var layout in gameObject.GetComponents<LayoutGroup>())
                Object.DestroyImmediate(layout);
        }
    }
}
