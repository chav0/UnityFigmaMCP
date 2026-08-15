using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    [Serializable]
    public class HorizontalGroupPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        [FormerlySerializedAs("turnOn")]
        [SerializeField] private bool startEnabled;

        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.layoutMode != FigmaLayoutMode.HORIZONTAL)
                return;

            var group = context.GameObject.GetComponent<HorizontalLayoutGroup>();
            if (group == null)
                group = context.GameObject.AddComponent<HorizontalLayoutGroup>();
            group.enabled = startEnabled;
            group.spacing = figmaObject.itemSpacing;
            group.padding = new RectOffset(
                (int)figmaObject.paddingLeft,
                (int)figmaObject.paddingRight,
                (int)figmaObject.paddingTop,
                (int)figmaObject.paddingBottom);

            var isSpaceBetween = figmaObject.primaryAxisAlignItems == FigmaLayoutAlign.SPACE_BETWEEN;

            group.childAlignment = ResolveAlignment(figmaObject, true);
            group.childControlHeight = false;
            group.childControlWidth = false;
            group.childForceExpandWidth = isSpaceBetween;
            group.childForceExpandHeight = isSpaceBetween;
        }
    }
}
