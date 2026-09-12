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
                HFit = contentSizeFitter.horizontalFit.ToString(),
                VFit = contentSizeFitter.verticalFit.ToString()
            };
        }

        public override void Write(ContentSizeFitter contentSizeFitter, ContentSizeFitterComponent dto)
        {
            if (!string.IsNullOrEmpty(dto.HFit) &&
                Enum.TryParse<ContentSizeFitter.FitMode>(dto.HFit, true, out var horizontalFit))
                contentSizeFitter.horizontalFit = horizontalFit;

            if (!string.IsNullOrEmpty(dto.VFit) &&
                Enum.TryParse<ContentSizeFitter.FitMode>(dto.VFit, true, out var verticalFit))
                contentSizeFitter.verticalFit = verticalFit;
        }

        protected override void Assign(UnityObject target, ContentSizeFitterComponent dto) => target.Fitter = dto;
    }
}
