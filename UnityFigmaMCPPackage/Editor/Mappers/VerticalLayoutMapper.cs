using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class VerticalLayoutMapper : LayoutGroupMapper<VerticalLayoutGroup, VerticalLayoutComponent>
    {
        public override VerticalLayoutComponent Read(VerticalLayoutGroup v)
        {
            return new VerticalLayoutComponent
            {
                Spacing = v.spacing,
                Align = v.childAlignment.ToString(),
                ExpandW = v.childForceExpandWidth,
                ExpandH = v.childForceExpandHeight,
                ControlW = v.childControlWidth,
                ControlH = v.childControlHeight,
                Pad = new[] { (float)v.padding.left, v.padding.right, v.padding.top, v.padding.bottom }
            };
        }

        public override void Write(VerticalLayoutGroup v, VerticalLayoutComponent dto)
        {
            if (dto.Spacing.HasValue)
                v.spacing = dto.Spacing.Value;

            if (dto.ExpandW.HasValue)
                v.childForceExpandWidth = dto.ExpandW.Value;

            if (dto.ExpandH.HasValue)
                v.childForceExpandHeight = dto.ExpandH.Value;

            if (dto.ControlW.HasValue)
                v.childControlWidth = dto.ControlW.Value;

            if (dto.ControlH.HasValue)
                v.childControlHeight = dto.ControlH.Value;

            ApplyAlignment(v, dto.Align);
            ApplyPadding(v, dto.Pad);
        }

        protected override void Assign(UnityObject target, VerticalLayoutComponent dto) => target.VLayout = dto;
    }
}
