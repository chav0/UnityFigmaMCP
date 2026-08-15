using System;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    [Serializable]
    public class RectTransformPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var gameObject = context.GameObject;
            var figmaObject = context.FigmaObject;
            var parent = context.ParentTransform;
            var rootFrame = context.RootFrame;

            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null)
                rectTransform = gameObject.AddComponent<RectTransform>();

            var xPos = 0f;
            var yPos = 0f;
            var xAnchorMin = 0f;
            var yAnchorMin = 0f;
            var xAnchorMax = 0f;
            var yAnchorMax = 0f;
            var xPivot = 0f;
            var yPivot = 0f;

            var objectX = figmaObject.absoluteBoundingBox.x ?? 0;
            var objectY = figmaObject.absoluteBoundingBox.y ?? 0;

            var objectWidth = figmaObject.absoluteBoundingBox.width ?? 0;
            var objectHeight = figmaObject.absoluteBoundingBox.height ?? 0;

            var parentFigma = context.ParentFigmaObject;
            float refX, refY, refWidth, refHeight;

            if (parentFigma != null)
            {
                refX = parentFigma.absoluteBoundingBox.x ?? 0;
                refY = parentFigma.absoluteBoundingBox.y ?? 0;
                refWidth = parentFigma.absoluteBoundingBox.width ?? 0;
                refHeight = parentFigma.absoluteBoundingBox.height ?? 0;
            }
            else
            {
                refX = rootFrame.absoluteBoundingBox.x ?? 0;
                refY = rootFrame.absoluteBoundingBox.y ?? 0;
                refWidth = 0;
                refHeight = 0;
            }

            switch (figmaObject.constraints.horizontal)
            {
                case FigmaSeparatedConstraints.LEFT:
                    xPivot = 0f;
                    xAnchorMin = 0f;
                    xAnchorMax = 0f;
                    break;
                case FigmaSeparatedConstraints.RIGHT:
                    xPivot = 1f;
                    xAnchorMin = 1f;
                    xAnchorMax = 1f;
                    break;
                case FigmaSeparatedConstraints.CENTER:
                    xPivot = 0.5f;
                    xAnchorMin = 0.5f;
                    xAnchorMax = 0.5f;
                    break;
                case FigmaSeparatedConstraints.LEFT_RIGHT:
                    xPivot = 0.5f;
                    xAnchorMin = 0f;
                    xAnchorMax = 1f;
                    break;
                case FigmaSeparatedConstraints.SCALE:
                    Debug.LogWarning($"Scale horizontal constraints found in {figmaObject.name}. Not supported in Unity. Changed to Left-Right.");
                    xPivot = 0.5f;
                    xAnchorMin = 0f;
                    xAnchorMax = 1f;
                    break;
            }

            switch (figmaObject.constraints.vertical)
            {
                case FigmaSeparatedConstraints.BOTTOM:
                    yPivot = 0f;
                    yAnchorMin = 0f;
                    yAnchorMax = 0f;
                    break;
                case FigmaSeparatedConstraints.TOP:
                    yPivot = 1f;
                    yAnchorMin = 1f;
                    yAnchorMax = 1f;
                    break;
                case FigmaSeparatedConstraints.CENTER:
                    yPivot = 0.5f;
                    yAnchorMin = 0.5f;
                    yAnchorMax = 0.5f;
                    break;
                case FigmaSeparatedConstraints.TOP_BOTTOM:
                    yPivot = 0.5f;
                    yAnchorMin = 0f;
                    yAnchorMax = 1f;
                    break;
                case FigmaSeparatedConstraints.SCALE:
                    Debug.LogWarning($"Scale vertical constraints found in {figmaObject.name}. Not supported in Unity. Changed to Top-Bottom.");
                    yPivot = 0.5f;
                    yAnchorMin = 0f;
                    yAnchorMax = 1f;
                    break;
            }

            xPos = (objectX + objectWidth * xPivot) - (refX + refWidth * xPivot);
            yPos = (refY + refHeight * (1 - yPivot)) - (objectY + objectHeight * (1 - yPivot));

            rectTransform.pivot = new Vector2(xPivot, yPivot);
            rectTransform.anchorMin = new Vector2(xAnchorMin, yAnchorMin);
            rectTransform.anchorMax = new Vector2(xAnchorMax, yAnchorMax);
            rectTransform.anchoredPosition = new Vector2(xPos, yPos);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, objectWidth);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, objectHeight);

            if (parent != null && parentFigma == null)
                rectTransform.SetParent(parent, true);
        }
    }
}
