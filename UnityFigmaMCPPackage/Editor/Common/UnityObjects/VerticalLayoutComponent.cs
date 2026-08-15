using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class VerticalLayoutComponent
    {
        [Description("Spacing between children")] public float? Spacing { get; set; }
        [Description("Child alignment (e.g. \"MiddleCenter\", \"UpperLeft\")")] public string ChildAlignment { get; set; }
        [Description("Force children to expand to fill extra horizontal space")] public bool? ChildForceExpandWidth { get; set; }
        [Description("Force children to expand to fill extra vertical space")] public bool? ChildForceExpandHeight { get; set; }
        [Description("Control the width of children")] public bool? ChildControlWidth { get; set; }
        [Description("Control the height of children")] public bool? ChildControlHeight { get; set; }
        [Description("Left padding")] public float? PaddingLeft { get; set; }
        [Description("Right padding")] public float? PaddingRight { get; set; }
        [Description("Top padding")] public float? PaddingTop { get; set; }
        [Description("Bottom padding")] public float? PaddingBottom { get; set; }
    }
}
