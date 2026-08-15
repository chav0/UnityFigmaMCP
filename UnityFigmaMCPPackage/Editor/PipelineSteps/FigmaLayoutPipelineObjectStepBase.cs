using System;
using UnityEngine;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    [Serializable]
    public abstract class FigmaLayoutPipelineObjectStepBase
    {
        public abstract void Execute(ObjectLayoutContext context);

        protected static TextAnchor ResolveAlignment(FigmaObject figmaObject, bool primaryIsHorizontal)
        {
            var primaryAxis = figmaObject.primaryAxisAlignItems;
            var counterAxis = figmaObject.counterAxisAlignItems;

            int horizontal, vertical;

            if (primaryIsHorizontal)
            {
                horizontal = ToPosition(primaryAxis);
                vertical = ToPosition(counterAxis);
            }
            else
            {
                vertical = ToPosition(primaryAxis);
                horizontal = ToPosition(counterAxis);
            }

            return (TextAnchor)(vertical * 3 + horizontal);
        }

        private static int ToPosition(FigmaLayoutAlign align)
        {
            switch (align)
            {
                case FigmaLayoutAlign.FIXED: return 0;
                case FigmaLayoutAlign.CENTER: return 1;
                case FigmaLayoutAlign.MAX: return 2;
                case FigmaLayoutAlign.SPACE_BETWEEN: return 1;
                default: return 0;
            }
        }
    }
}
