using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    [Serializable]
    public class ContentSizeFitterPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        [FormerlySerializedAs("turnOn")]
        [SerializeField] private bool startEnabled;

        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.layoutMode == FigmaLayoutMode.NONE)
                return;

            if (figmaObject.counterAxisSizingMode == FigmaSizing.FIXED &&
                figmaObject.primaryAxisSizingMode == FigmaSizing.FIXED)
                return;

            var contentSizeFitter = context.GameObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
                contentSizeFitter = context.GameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.enabled = startEnabled;
            switch (figmaObject.layoutMode)
            {
                case FigmaLayoutMode.HORIZONTAL:
                case FigmaLayoutMode.GRID:
                    contentSizeFitter.horizontalFit = figmaObject.primaryAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained;

                    contentSizeFitter.verticalFit = figmaObject.counterAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained;
                    break;
                case FigmaLayoutMode.VERTICAL:
                    contentSizeFitter.verticalFit = figmaObject.primaryAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained;

                    contentSizeFitter.horizontalFit = figmaObject.counterAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained;
                    break;
            }
        }
    }
}
