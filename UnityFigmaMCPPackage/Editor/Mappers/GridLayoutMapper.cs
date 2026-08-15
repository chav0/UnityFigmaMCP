using System;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class GridLayoutMapper : LayoutGroupMapper<GridLayoutGroup, GridLayoutComponent>
    {
        public override GridLayoutComponent Read(GridLayoutGroup gridLayoutGroup)
        {
            return new GridLayoutComponent
            {
                CellSizeX = gridLayoutGroup.cellSize.x,
                CellSizeY = gridLayoutGroup.cellSize.y,
                SpacingX = gridLayoutGroup.spacing.x,
                SpacingY = gridLayoutGroup.spacing.y,
                StartCorner = gridLayoutGroup.startCorner.ToString(),
                StartAxis = gridLayoutGroup.startAxis.ToString(),
                ChildAlignment = gridLayoutGroup.childAlignment.ToString(),
                Constraint = gridLayoutGroup.constraint.ToString(),
                ConstraintCount = gridLayoutGroup.constraintCount,
                PaddingLeft = gridLayoutGroup.padding.left,
                PaddingRight = gridLayoutGroup.padding.right,
                PaddingTop = gridLayoutGroup.padding.top,
                PaddingBottom = gridLayoutGroup.padding.bottom
            };
        }

        public override void Write(GridLayoutGroup gridLayoutGroup, GridLayoutComponent dto)
        {
            if (dto.CellSizeX.HasValue || dto.CellSizeY.HasValue)
                gridLayoutGroup.cellSize = new Vector2(
                    dto.CellSizeX ?? gridLayoutGroup.cellSize.x,
                    dto.CellSizeY ?? gridLayoutGroup.cellSize.y);

            if (dto.SpacingX.HasValue || dto.SpacingY.HasValue)
                gridLayoutGroup.spacing = new Vector2(
                    dto.SpacingX ?? gridLayoutGroup.spacing.x,
                    dto.SpacingY ?? gridLayoutGroup.spacing.y);

            if (!string.IsNullOrEmpty(dto.StartCorner) && Enum.TryParse<GridLayoutGroup.Corner>(dto.StartCorner, true, out var corner))
                gridLayoutGroup.startCorner = corner;

            if (!string.IsNullOrEmpty(dto.StartAxis) && Enum.TryParse<GridLayoutGroup.Axis>(dto.StartAxis, true, out var axis))
                gridLayoutGroup.startAxis = axis;

            if (!string.IsNullOrEmpty(dto.Constraint) && Enum.TryParse<GridLayoutGroup.Constraint>(dto.Constraint, true, out var constraint))
                gridLayoutGroup.constraint = constraint;

            if (dto.ConstraintCount.HasValue)
                gridLayoutGroup.constraintCount = dto.ConstraintCount.Value;

            ApplyAlignment(gridLayoutGroup, dto.ChildAlignment);
            ApplyPadding(gridLayoutGroup, dto.PaddingLeft, dto.PaddingRight, dto.PaddingTop, dto.PaddingBottom);
        }

        protected override void Assign(UnityObject target, GridLayoutComponent dto) => target.GridLayout = dto;
    }
}
