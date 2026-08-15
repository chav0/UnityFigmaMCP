using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class ImageMapper : ComponentMapper<Image, ImageComponent>
    {
        public override ImageComponent Read(Image image)
        {
            string spritePath = null;
            if (image.sprite != null)
                spritePath = AssetDatabase.GetAssetPath(image.sprite);

            return new ImageComponent
            {
                SpriteName = image.sprite != null ? image.sprite.name : null,
                SpritePath = spritePath,
                Color = ColorUtility.ToHtmlStringRGBA(image.color),
                Type = image.type.ToString(),
                RaycastTarget = image.raycastTarget
            };
        }

        public override void Write(Image image, ImageComponent dto)
        {
            if (!string.IsNullOrEmpty(dto.SpritePath))
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(dto.SpritePath);
                if (sprite != null)
                    image.sprite = sprite;
            }

            if (!string.IsNullOrEmpty(dto.Color) && ColorUtility.TryParseHtmlString(dto.Color, out var color))
                image.color = color;

            if (!string.IsNullOrEmpty(dto.Type) && Enum.TryParse<Image.Type>(dto.Type, true, out var type))
                image.type = type;

            if (dto.RaycastTarget.HasValue)
                image.raycastTarget = dto.RaycastTarget.Value;
        }

        protected override void Assign(UnityObject target, ImageComponent dto) => target.Image = dto;
    }
}
