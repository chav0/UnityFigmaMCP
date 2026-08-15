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
                FontSize = text.fontSize,
                FontFamily = text.font != null ? text.font.faceInfo.familyName : null,
                FontStyle = text.font != null ? text.font.faceInfo.styleName : null,
                Color = ColorUtility.ToHtmlStringRGBA(text.color),
                Alignment = text.alignment.ToString(),
                AutoSize = text.enableAutoSizing
            };
        }

        public override void Write(TextMeshProUGUI text, TextComponent dto)
        {
            if (dto.Text != null)
                text.text = dto.Text;

            if (dto.FontSize.HasValue)
                text.fontSize = dto.FontSize.Value;

            if (!string.IsNullOrEmpty(dto.FontFamily))
            {
                var style = !string.IsNullOrEmpty(dto.FontStyle) ? dto.FontStyle : FontHelper.DefaultStyle;
                text.font = FontHelper.FindFont(dto.FontFamily, style);
            }

            if (!string.IsNullOrEmpty(dto.Color) && ColorUtility.TryParseHtmlString(dto.Color, out var color))
                text.color = color;

            if (!string.IsNullOrEmpty(dto.Alignment) && Enum.TryParse<TextAlignmentOptions>(dto.Alignment, true, out var alignment))
                text.alignment = alignment;

            if (dto.AutoSize.HasValue)
                text.enableAutoSizing = dto.AutoSize.Value;
        }

        protected override void Assign(UnityObject target, TextComponent dto) => target.Text = dto;
    }
}
