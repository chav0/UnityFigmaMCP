using System;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    [Serializable]
    public class GridPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.layoutMode != FigmaLayoutMode.GRID)
                return;

            var grid = context.GameObject.GetComponent<GridLayoutGroup>();
            if (grid == null)
                grid = context.GameObject.AddComponent<GridLayoutGroup>();

            grid.padding = new RectOffset(
                (int)figmaObject.paddingLeft,
                (int)figmaObject.paddingRight,
                (int)figmaObject.paddingTop,
                (int)figmaObject.paddingBottom);

            grid.spacing = new Vector2(figmaObject.gridColumnGap, figmaObject.gridRowGap);

            if (figmaObject.gridColumnCount > 0)
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = figmaObject.gridColumnCount;
            }
            else if (figmaObject.gridRowCount > 0)
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                grid.constraintCount = figmaObject.gridRowCount;
            }
            else
            {
                grid.constraint = GridLayoutGroup.Constraint.Flexible;
            }

            grid.cellSize = CalculateCellSize(figmaObject);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = ResolveAlignment(figmaObject, true);
        }

        private static Vector2 CalculateCellSize(FigmaObject figmaObject)
        {
            var boundingBox = figmaObject.absoluteBoundingBox;
            if (boundingBox.width == null || boundingBox.height == null)
                return new Vector2(100, 100);

            var columnCount = figmaObject.gridColumnCount > 0 ? figmaObject.gridColumnCount : 1;
            var rowCount = figmaObject.gridRowCount > 0 ? figmaObject.gridRowCount : 1;

            var totalWidth = boundingBox.width.Value - figmaObject.paddingLeft - figmaObject.paddingRight;
            var totalHeight = boundingBox.height.Value - figmaObject.paddingTop - figmaObject.paddingBottom;

            var cellWidth = (totalWidth - figmaObject.gridColumnGap * (columnCount - 1)) / columnCount;
            var cellHeight = (totalHeight - figmaObject.gridRowGap * (rowCount - 1)) / rowCount;

            return new Vector2(Mathf.Max(cellWidth, 1f), Mathf.Max(cellHeight, 1f));
        }
    }
}
