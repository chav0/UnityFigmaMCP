using System;
using TMPro;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
	[Serializable]
    public class TextPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.type != FigmaObjectType.TEXT)
                return;
            
            var text = context.GameObject.GetComponent<TextMeshProUGUI>();
            if (text == null)
                text = context.GameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = figmaObject.style.fontSize;
            text.color = FigmaColorHelper.CalculateColor(figmaObject.fills);
            text.text = figmaObject.characters;
            text.alignment = FontHelper.GetAlignment(figmaObject.style.textAlignHorizontal, figmaObject.style.textAlignVertical);
            text.characterSpacing = figmaObject.style.letterSpacing / figmaObject.style.fontSize * 100;
            text.paragraphSpacing = figmaObject.style.paragraphSpacing / figmaObject.style.fontSize * 100;
            text.fontStyle = figmaObject.style.textCase.ToFontStyle();

            try
            {
                text.font = FontHelper.FindFontFromFigma(figmaObject.style.fontFamily, figmaObject.style.fontPostScriptName);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
