using System;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    [Serializable]
    public class ImagePipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var gameObject = context.GameObject;
            var figmaObject = context.FigmaObject;
            if ((figmaObject.type & FigmaObjectType.GRAPHIC) == FigmaObjectType.NONE)
                return;

            if (figmaObject.isMask)
            {
                switch (figmaObject.type)
                {
                    case FigmaObjectType.CANVAS:
                    case FigmaObjectType.FRAME:
                    case FigmaObjectType.RECTANGLE:
                    case FigmaObjectType.IMAGE:
                        if (gameObject.GetComponent<RectMask2D>() == null)
                            gameObject.AddComponent<RectMask2D>();
                        return;
                    case FigmaObjectType.LINE:
                    case FigmaObjectType.REGULAR_POLYGON:
                    case FigmaObjectType.VECTOR:
                    case FigmaObjectType.STAR:
                    case FigmaObjectType.ELLIPSE:
                        var mask = gameObject.GetComponent<Mask>();
                        if (mask == null)
                            mask = gameObject.AddComponent<Mask>();
                        mask.showMaskGraphic = false;
                        break;
                }
            }

            if (!FigmaColorHelper.NeedAddImage(figmaObject.fills))
                return;

            var image = gameObject.GetComponent<Image>();
            if (image == null)
                image = gameObject.AddComponent<Image>();

            var color = FigmaColorHelper.CalculateColor(figmaObject.fills);
            color = new Color(color.r, color.g, color.b, color.a * figmaObject.opacity.GetValueOrDefault(1f));
            image.color = color;

            var sprite = context.Assets?.ResolveSprite(figmaObject)?.Sprite;
            image.sprite = sprite;

            if (sprite != null)
                image.type = sprite.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple;

            if (figmaObject.name.ToLower().Contains("button"))
            {
                var button = gameObject.GetComponent<Button>();
                if (button == null)
                    button = gameObject.AddComponent<Button>();
                button.image = image;
            }
            else
            {
                image.raycastTarget = false;
            }
        }
    }
}
