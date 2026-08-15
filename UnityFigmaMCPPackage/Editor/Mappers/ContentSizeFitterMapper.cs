using System;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class ContentSizeFitterMapper : ComponentMapper<ContentSizeFitter, ContentSizeFitterComponent>
    {
        public override ContentSizeFitterComponent Read(ContentSizeFitter contentSizeFitter)
        {
            return new ContentSizeFitterComponent
            {
                HorizontalFit = contentSizeFitter.horizontalFit.ToString(),
                VerticalFit = contentSizeFitter.verticalFit.ToString()
            };
        }

        public override void Write(ContentSizeFitter contentSizeFitter, ContentSizeFitterComponent dto)
        {
            if (!string.IsNullOrEmpty(dto.HorizontalFit) &&
                Enum.TryParse<ContentSizeFitter.FitMode>(dto.HorizontalFit, true, out var horizontalFit))
                contentSizeFitter.horizontalFit = horizontalFit;

            if (!string.IsNullOrEmpty(dto.VerticalFit) &&
                Enum.TryParse<ContentSizeFitter.FitMode>(dto.VerticalFit, true, out var verticalFit))
                contentSizeFitter.verticalFit = verticalFit;
        }

        protected override void Assign(UnityObject target, ContentSizeFitterComponent dto) => target.ContentSizeFitter = dto;
    }
}
