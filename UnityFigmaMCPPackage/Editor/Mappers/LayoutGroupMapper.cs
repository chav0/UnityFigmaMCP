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

        protected static void ApplyPadding(LayoutGroup layoutGroup, float? left, float? right, float? top, float? bottom)
        {
            if (!left.HasValue && !right.HasValue && !top.HasValue && !bottom.HasValue)
                return;

            layoutGroup.padding = new RectOffset(
                left.HasValue ? (int)left.Value : layoutGroup.padding.left,
                right.HasValue ? (int)right.Value : layoutGroup.padding.right,
                top.HasValue ? (int)top.Value : layoutGroup.padding.top,
                bottom.HasValue ? (int)bottom.Value : layoutGroup.padding.bottom);
        }

        private static void RemoveAllLayouts(GameObject gameObject)
        {
            foreach (var layout in gameObject.GetComponents<LayoutGroup>())
                Object.DestroyImmediate(layout);
        }
    }
}
