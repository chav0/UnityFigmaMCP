using System;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class RectTransformMapper : ComponentMapper<RectTransform, RectTransformComponent>
    {
        public override RectTransformComponent Read(RectTransform rt)
        {
            return new RectTransformComponent
            {
                Anchors = new[] { rt.anchorMin.x, rt.anchorMin.y, rt.anchorMax.x, rt.anchorMax.y },
                Pivot = new[] { rt.pivot.x, rt.pivot.y },
                Size = new[] { rt.rect.width, rt.rect.height },
                Pos = new[] { rt.anchoredPosition.x, rt.anchoredPosition.y }
            };
        }

        public override void Write(RectTransform rt, RectTransformComponent dto)
        {
            if (dto.Anchors is { Length: 4 })
            {
                rt.anchorMin = new Vector2(dto.Anchors[0], dto.Anchors[1]);
                rt.anchorMax = new Vector2(dto.Anchors[2], dto.Anchors[3]);
            }

            if (dto.Pivot is { Length: 2 })
                rt.pivot = new Vector2(dto.Pivot[0], dto.Pivot[1]);

            if (dto.Size is { Length: 2 })
                rt.sizeDelta = new Vector2(dto.Size[0], dto.Size[1]);

            if (dto.Pos is { Length: 2 })
                rt.anchoredPosition = new Vector2(dto.Pos[0], dto.Pos[1]);
        }

        public override void Apply(GameObject gameObject, RectTransformComponent dto)
        {
            var rt = gameObject.GetComponent<RectTransform>();
            if (rt == null)
                throw new Exception("RectTransform not found");

            Write(rt, dto);
        }

        protected override void Assign(UnityObject target, RectTransformComponent dto) => target.Rect = dto;
    }
}
