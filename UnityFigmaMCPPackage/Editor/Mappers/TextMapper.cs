using System;
using TMPro;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class TextMapper : ComponentMapper<TextMeshProUGUI, TextComponent>
    {
        public override TextComponent Read(TextMeshProUGUI text)
        {
            return new TextComponent
            {
                Text = text.text,
                Size = text.fontSize,
                Font = text.font != null ? text.font.faceInfo.familyName : null,
                Style = text.font != null ? text.font.faceInfo.styleName : null,
                Color = ColorUtility.ToHtmlStringRGBA(text.color),
                Align = text.alignment.ToString(),
                Auto = text.enableAutoSizing
            };
        }

        public override void Write(TextMeshProUGUI text, TextComponent dto)
        {
            if (dto.Text != null)
                text.text = dto.Text;

            if (dto.Size.HasValue)
                text.fontSize = dto.Size.Value;

            if (!string.IsNullOrEmpty(dto.Font))
            {
                var style = !string.IsNullOrEmpty(dto.Style) ? dto.Style : FontHelper.DefaultStyle;
                text.font = FontHelper.FindFont(dto.Font, style);
            }

            if (!string.IsNullOrEmpty(dto.Color) && ColorUtility.TryParseHtmlString(dto.Color, out var color))
                text.color = color;

            if (!string.IsNullOrEmpty(dto.Align) && Enum.TryParse<TextAlignmentOptions>(dto.Align, true, out var alignment))
                text.alignment = alignment;

            if (dto.Auto.HasValue)
                text.enableAutoSizing = dto.Auto.Value;
        }

        protected override void Assign(UnityObject target, TextComponent dto) => target.Text = dto;
    }
}
