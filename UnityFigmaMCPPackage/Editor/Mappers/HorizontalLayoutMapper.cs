using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class HorizontalLayoutMapper : LayoutGroupMapper<HorizontalLayoutGroup, HorizontalLayoutComponent>
    {
        public override HorizontalLayoutComponent Read(HorizontalLayoutGroup h)
        {
            return new HorizontalLayoutComponent
            {
                Spacing = h.spacing,
                Align = h.childAlignment.ToString(),
                ExpandW = h.childForceExpandWidth,
                ExpandH = h.childForceExpandHeight,
                ControlW = h.childControlWidth,
                ControlH = h.childControlHeight,
                Pad = new[] { (float)h.padding.left, h.padding.right, h.padding.top, h.padding.bottom }
            };
        }

        public override void Write(HorizontalLayoutGroup h, HorizontalLayoutComponent dto)
        {
            if (dto.Spacing.HasValue)
                h.spacing = dto.Spacing.Value;

            if (dto.ExpandW.HasValue)
                h.childForceExpandWidth = dto.ExpandW.Value;

            if (dto.ExpandH.HasValue)
                h.childForceExpandHeight = dto.ExpandH.Value;

            if (dto.ControlW.HasValue)
                h.childControlWidth = dto.ControlW.Value;

            if (dto.ControlH.HasValue)
                h.childControlHeight = dto.ControlH.Value;

            ApplyAlignment(h, dto.Align);
            ApplyPadding(h, dto.Pad);
        }

        protected override void Assign(UnityObject target, HorizontalLayoutComponent dto) => target.HLayout = dto;
    }
}
