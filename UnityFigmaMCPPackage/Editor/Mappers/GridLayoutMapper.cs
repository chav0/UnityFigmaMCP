using System;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class GridLayoutMapper : LayoutGroupMapper<GridLayoutGroup, GridLayoutComponent>
    {
        public override GridLayoutComponent Read(GridLayoutGroup g)
        {
            return new GridLayoutComponent
            {
                Cell = new[] { g.cellSize.x, g.cellSize.y },
                Spacing = new[] { g.spacing.x, g.spacing.y },
                Corner = g.startCorner.ToString(),
                Axis = g.startAxis.ToString(),
                Align = g.childAlignment.ToString(),
                Constraint = g.constraint.ToString(),
                Count = g.constraintCount,
                Pad = new[] { (float)g.padding.left, g.padding.right, g.padding.top, g.padding.bottom }
            };
        }

        public override void Write(GridLayoutGroup g, GridLayoutComponent dto)
        {
            if (dto.Cell is { Length: 2 })
                g.cellSize = new Vector2(dto.Cell[0], dto.Cell[1]);

            if (dto.Spacing is { Length: 2 })
                g.spacing = new Vector2(dto.Spacing[0], dto.Spacing[1]);

            if (!string.IsNullOrEmpty(dto.Corner) && Enum.TryParse<GridLayoutGroup.Corner>(dto.Corner, true, out var corner))
                g.startCorner = corner;

            if (!string.IsNullOrEmpty(dto.Axis) && Enum.TryParse<GridLayoutGroup.Axis>(dto.Axis, true, out var axis))
                g.startAxis = axis;

            if (!string.IsNullOrEmpty(dto.Constraint) && Enum.TryParse<GridLayoutGroup.Constraint>(dto.Constraint, true, out var constraint))
                g.constraint = constraint;

            if (dto.Count.HasValue)
                g.constraintCount = dto.Count.Value;

            ApplyAlignment(g, dto.Align);
            ApplyPadding(g, dto.Pad);
        }

        protected override void Assign(UnityObject target, GridLayoutComponent dto) => target.Grid = dto;
    }
}
