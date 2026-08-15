using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal static class FigmaColorHelper
    {
        internal static bool NeedAddImage(IReadOnlyList<FigmaFill> fills)
        {
            return fills.Any(fill => !fill.visible.HasValue || fill.visible.Value);
        }

        internal static Color CalculateColor(IReadOnlyList<FigmaFill> fills)
        {
            var color = new Color();

            if (fills == null || fills.Count == 0)
                return color;

            var firstFill = fills[0];
            if (!firstFill.visible.HasValue || firstFill.visible.Value)
            {
                switch (firstFill.type)
                {
                    case "SOLID":
                        color = new Color(firstFill.color.r, firstFill.color.g, firstFill.color.b,
                            firstFill.opacity.GetValueOrDefault(firstFill.color.a));
                        break;
                    case "IMAGE":
                        color = Color.white;
                        break;
                }
            }

            if (fills.Count <= 1)
                return color;

            for (var i = 1; i < fills.Count; i++)
            {
                var fill = fills[i];

                if (fill.visible.HasValue && !fill.visible.Value)
                    continue;

                switch (fill.type)
                {
                    case "SOLID":
                        color = ApplyBlend(color, fill);
                        break;
                    default:
                        Debug.LogWarning($"Unity base UI tools does not support gradients like {fill.type}. Try ask your UI designer fix it :)");
                        break;
                }
            }

            return color;
        }

        private static Color ApplyBlend(Color baseColor, FigmaFill fill)
        {
            var blendMode = fill.blendMode;

            Func<float, float, float, float> blend = blendMode switch
            {
                FigmaBlendMode.MULTIPLY => Multiply,
                FigmaBlendMode.OVERLAY => Overlay,
                FigmaBlendMode.SCREEN => Screen,
                _ => AlphaBlending
            };

            var opacity = fill.opacity.GetValueOrDefault(fill.color.a);
            var red = blend(baseColor.r, fill.color.r, opacity);
            var green = blend(baseColor.g, fill.color.g, opacity);
            var blue = blend(baseColor.b, fill.color.b, opacity);
            var alpha = baseColor.a + (1f - baseColor.a) * opacity;

            return new Color(red, green, blue, alpha);
        }

        private static float Overlay(float baseChannel, float blendChannel, float opacity)
        {
            return baseChannel < 0.5f
                ? Multiply(baseChannel, blendChannel, opacity)
                : Screen(baseChannel, blendChannel, opacity);
        }

        private static float Multiply(float a, float b, float alpha)
        {
            return AlphaBlending(a, a * b, alpha);
        }

        private static float Screen(float a, float b, float alpha)
        {
            return AlphaBlending(a, 1f - (1f - a) * (1f - b), alpha);
        }

        private static float AlphaBlending(float a, float b, float alpha)
        {
            return b * alpha + a * (1f - alpha);
        }
    }
}
