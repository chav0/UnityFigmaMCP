using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class GridLayoutComponent
    {
        [Description("Cell size [width, height] in pixels")]
        public float[] Cell { get; set; }

        [Description("Spacing [x, y] between cells in pixels")]
        public float[] Spacing { get; set; }

        [Description("Padding [left, right, top, bottom] in pixels")]
        public float[] Pad { get; set; }

        [Description("Corner the grid starts from: \"UpperLeft\", \"UpperRight\", \"LowerLeft\" or \"LowerRight\"")]
        public string Corner { get; set; }

        [Description("Axis the grid fills first: \"Horizontal\" or \"Vertical\"")]
        public string Axis { get; set; }

        [Description("Child alignment: \"UpperLeft\", \"MiddleCenter\", etc.")]
        public string Align { get; set; }

        [Description("Constraint: \"Flexible\", \"FixedColumnCount\" or \"FixedRowCount\"")]
        public string Constraint { get; set; }

        [Description("Column or row count when Constraint is fixed")]
        public int? Count { get; set; }
    }
}
