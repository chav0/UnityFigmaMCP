using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class GridLayoutComponent
    {
        [Description("Cell width in pixels")] public float? CellSizeX { get; set; }
        [Description("Cell height in pixels")] public float? CellSizeY { get; set; }
        [Description("Horizontal spacing between cells")] public float? SpacingX { get; set; }
        [Description("Vertical spacing between cells")] public float? SpacingY { get; set; }
        [Description("Corner the grid starts from: \"UpperLeft\", \"UpperRight\", \"LowerLeft\" or \"LowerRight\"")] public string StartCorner { get; set; }
        [Description("Axis the grid fills first: \"Horizontal\" or \"Vertical\"")] public string StartAxis { get; set; }
        [Description("Child alignment (e.g. \"MiddleCenter\", \"UpperLeft\")")] public string ChildAlignment { get; set; }
        [Description("Grid constraint: \"Flexible\", \"FixedColumnCount\" or \"FixedRowCount\"")] public string Constraint { get; set; }
        [Description("Column or row count when Constraint is fixed")] public int? ConstraintCount { get; set; }
        [Description("Left padding")] public float? PaddingLeft { get; set; }
        [Description("Right padding")] public float? PaddingRight { get; set; }
        [Description("Top padding")] public float? PaddingTop { get; set; }
        [Description("Bottom padding")] public float? PaddingBottom { get; set; }
    }
}
