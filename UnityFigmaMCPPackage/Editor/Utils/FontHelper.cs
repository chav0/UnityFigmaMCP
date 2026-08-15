using System;
using TMPro;
using UnityEditor;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    public static class FontHelper
    {
        public const string DefaultStyle = "Regular";

        public static TMP_FontAsset FindFontFromFigma(string fontFamily, string fontPostScript)
        {
            var postScriptParts = fontPostScript.Split('-');
            var fontStyle = postScriptParts[1];

            return FindFont(fontFamily, fontStyle);
        }

        public static TMP_FontAsset FindFont(string fontFamily, string fontStyle)
        {
            var guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] {"Assets"});
            foreach (var guid in guids)
            {
                var font = (TMP_FontAsset) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(TMP_FontAsset));
                if (font.faceInfo.familyName == fontFamily && font.faceInfo.styleName == fontStyle)
                    return font;
            }

            throw new Exception($"Font {fontFamily}-{fontStyle} not found!");
        }
        
        public static TextAlignmentOptions GetAlignment(FigmaAlign horizontal, FigmaAlign vertical)
        {
            switch (horizontal)
            {
                case FigmaAlign.CENTER:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER: return TextAlignmentOptions.Center;
                        case FigmaAlign.TOP: return TextAlignmentOptions.Top; 
                        case FigmaAlign.BOTTOM: return TextAlignmentOptions.Bottom;
                    }
                    break;
                case FigmaAlign.LEFT:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER: return TextAlignmentOptions.Left;
                        case FigmaAlign.TOP: return TextAlignmentOptions.TopLeft; 
                        case FigmaAlign.BOTTOM: return TextAlignmentOptions.BottomLeft;
                    }
                    break;
                case FigmaAlign.RIGHT:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER: return TextAlignmentOptions.Right;
                        case FigmaAlign.TOP: return TextAlignmentOptions.TopRight; 
                        case FigmaAlign.BOTTOM: return TextAlignmentOptions.BottomRight;
                    }
                    break;
                case FigmaAlign.JUSTIFIED:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER: return TextAlignmentOptions.Justified;
                        case FigmaAlign.TOP: return TextAlignmentOptions.TopJustified; 
                        case FigmaAlign.BOTTOM: return TextAlignmentOptions.BottomJustified;
                    }
                    break;
            }

            return TextAlignmentOptions.Baseline; 
        }
        
        public static FontStyles ToFontStyle(this TextCase textCase)
        {
            switch (textCase)
            {
                case TextCase.NONE: return FontStyles.Normal;
                case TextCase.UPPER: return FontStyles.UpperCase;
                case TextCase.LOWER: return FontStyles.LowerCase;
                case TextCase.TITLE: return FontStyles.Normal;
            }

            return FontStyles.Normal; 
        }
    }
}