using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class HorizontalLayoutMapper : LayoutGroupMapper<HorizontalLayoutGroup, HorizontalLayoutComponent>
    {
        public override HorizontalLayoutComponent Read(HorizontalLayoutGroup horizontalLayoutGroup)
        {
            return new HorizontalLayoutComponent
            {
                Spacing = horizontalLayoutGroup.spacing,
                ChildAlignment = horizontalLayoutGroup.childAlignment.ToString(),
                ChildForceExpandWidth = horizontalLayoutGroup.childForceExpandWidth,
                ChildForceExpandHeight = horizontalLayoutGroup.childForceExpandHeight,
                ChildControlWidth = horizontalLayoutGroup.childControlWidth,
                ChildControlHeight = horizontalLayoutGroup.childControlHeight,
                PaddingLeft = horizontalLayoutGroup.padding.left,
                PaddingRight = horizontalLayoutGroup.padding.right,
                PaddingTop = horizontalLayoutGroup.padding.top,
                PaddingBottom = horizontalLayoutGroup.padding.bottom
            };
        }

        public override void Write(HorizontalLayoutGroup horizontalLayoutGroup, HorizontalLayoutComponent dto)
        {
            if (dto.Spacing.HasValue) 
                horizontalLayoutGroup.spacing = dto.Spacing.Value;

            if (dto.ChildForceExpandWidth.HasValue) 
                horizontalLayoutGroup.childForceExpandWidth = dto.ChildForceExpandWidth.Value;
            
            if (dto.ChildForceExpandHeight.HasValue) 
                horizontalLayoutGroup.childForceExpandHeight = dto.ChildForceExpandHeight.Value;
            
            if (dto.ChildControlWidth.HasValue) 
                horizontalLayoutGroup.childControlWidth = dto.ChildControlWidth.Value;
            
            if (dto.ChildControlHeight.HasValue) 
                horizontalLayoutGroup.childControlHeight = dto.ChildControlHeight.Value;

            ApplyAlignment(horizontalLayoutGroup, dto.ChildAlignment);
            ApplyPadding(horizontalLayoutGroup, dto.PaddingLeft, dto.PaddingRight, dto.PaddingTop, dto.PaddingBottom);
        }

        protected override void Assign(UnityObject target, HorizontalLayoutComponent dto) => target.HorizontalLayout = dto;
    }
}
