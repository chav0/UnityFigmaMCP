using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class VerticalLayoutComponent
    {
        [Description("Spacing between children in pixels")]
        public float? Spacing { get; set; }

        [Description("Child alignment: \"UpperLeft\", \"MiddleCenter\", etc.")]
        public string Align { get; set; }

        [Description("Force children to expand horizontally")]
        public bool? ExpandW { get; set; }

        [Description("Force children to expand vertically")]
        public bool? ExpandH { get; set; }

        [Description("Control children's width")]
        public bool? ControlW { get; set; }

        [Description("Control children's height")]
        public bool? ControlH { get; set; }

        [Description("Padding [left, right, top, bottom] in pixels")]
        public float[] Pad { get; set; }
    }
}
