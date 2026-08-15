using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class VerticalLayoutMapper : LayoutGroupMapper<VerticalLayoutGroup, VerticalLayoutComponent>
    {
        public override VerticalLayoutComponent Read(VerticalLayoutGroup verticalLayoutGroup)
        {
            return new VerticalLayoutComponent
            {
                Spacing = verticalLayoutGroup.spacing,
                ChildAlignment = verticalLayoutGroup.childAlignment.ToString(),
                ChildForceExpandWidth = verticalLayoutGroup.childForceExpandWidth,
                ChildForceExpandHeight = verticalLayoutGroup.childForceExpandHeight,
                ChildControlWidth = verticalLayoutGroup.childControlWidth,
                ChildControlHeight = verticalLayoutGroup.childControlHeight,
                PaddingLeft = verticalLayoutGroup.padding.left,
                PaddingRight = verticalLayoutGroup.padding.right,
                PaddingTop = verticalLayoutGroup.padding.top,
                PaddingBottom = verticalLayoutGroup.padding.bottom
            };
        }

        public override void Write(VerticalLayoutGroup verticalLayoutGroup, VerticalLayoutComponent dto)
        {
            if (dto.Spacing.HasValue) 
                verticalLayoutGroup.spacing = dto.Spacing.Value;

            if (dto.ChildForceExpandWidth.HasValue)
                verticalLayoutGroup.childForceExpandWidth = dto.ChildForceExpandWidth.Value;
            
            if (dto.ChildForceExpandHeight.HasValue)
                verticalLayoutGroup.childForceExpandHeight = dto.ChildForceExpandHeight.Value;
            
            if (dto.ChildControlWidth.HasValue) 
                verticalLayoutGroup.childControlWidth = dto.ChildControlWidth.Value;
            
            if (dto.ChildControlHeight.HasValue) 
                verticalLayoutGroup.childControlHeight = dto.ChildControlHeight.Value;

            ApplyAlignment(verticalLayoutGroup, dto.ChildAlignment);
            ApplyPadding(verticalLayoutGroup, dto.PaddingLeft, dto.PaddingRight, dto.PaddingTop, dto.PaddingBottom);
        }

        protected override void Assign(UnityObject target, VerticalLayoutComponent dto) => target.VerticalLayout = dto;
    }
}
