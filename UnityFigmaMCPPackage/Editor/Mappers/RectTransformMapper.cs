using System;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class RectTransformMapper : ComponentMapper<RectTransform, RectTransformComponent>
    {
        public override RectTransformComponent Read(RectTransform rectTransform)
        {
            return new RectTransformComponent
            {
                AnchorMinX = rectTransform.anchorMin.x,
                AnchorMinY = rectTransform.anchorMin.y,
                AnchorMaxX = rectTransform.anchorMax.x,
                AnchorMaxY = rectTransform.anchorMax.y,
                PivotX = rectTransform.pivot.x,
                PivotY = rectTransform.pivot.y,
                Width = rectTransform.rect.width,
                Height = rectTransform.rect.height,
                PosX = rectTransform.anchoredPosition.x,
                PosY = rectTransform.anchoredPosition.y
            };
        }

        public override void Write(RectTransform rectTransform, RectTransformComponent dto)
        {
            if (dto.AnchorMinX.HasValue || dto.AnchorMinY.HasValue)
                rectTransform.anchorMin = new Vector2(
                    dto.AnchorMinX ?? rectTransform.anchorMin.x,
                    dto.AnchorMinY ?? rectTransform.anchorMin.y);

            if (dto.AnchorMaxX.HasValue || dto.AnchorMaxY.HasValue)
                rectTransform.anchorMax = new Vector2(
                    dto.AnchorMaxX ?? rectTransform.anchorMax.x,
                    dto.AnchorMaxY ?? rectTransform.anchorMax.y);

            if (dto.PivotX.HasValue || dto.PivotY.HasValue)
                rectTransform.pivot = new Vector2(
                    dto.PivotX ?? rectTransform.pivot.x,
                    dto.PivotY ?? rectTransform.pivot.y);

            if (dto.Width.HasValue || dto.Height.HasValue)
                rectTransform.sizeDelta = new Vector2(
                    dto.Width ?? rectTransform.sizeDelta.x,
                    dto.Height ?? rectTransform.sizeDelta.y);

            if (dto.PosX.HasValue || dto.PosY.HasValue)
                rectTransform.anchoredPosition = new Vector2(
                    dto.PosX ?? rectTransform.anchoredPosition.x,
                    dto.PosY ?? rectTransform.anchoredPosition.y);
        }

        public override void Apply(GameObject gameObject, RectTransformComponent dto)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null)
                throw new Exception("RectTransform not found");
            
            Write(rectTransform, dto);
        }

        protected override void Assign(UnityObject target, RectTransformComponent dto) => target.RectTransform = dto;
    }
}
