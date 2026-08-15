using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class RectTransformComponent
    {
        [Description("Anchor min X (0..1)")] public float? AnchorMinX { get; set; }
        [Description("Anchor min Y (0..1)")] public float? AnchorMinY { get; set; }
        [Description("Anchor max X (0..1)")] public float? AnchorMaxX { get; set; }
        [Description("Anchor max Y (0..1)")] public float? AnchorMaxY { get; set; }
        [Description("Pivot X (0..1)")] public float? PivotX { get; set; }
        [Description("Pivot Y (0..1)")] public float? PivotY { get; set; }
        [Description("Width in pixels")] public float? Width { get; set; }
        [Description("Height in pixels")] public float? Height { get; set; }
        [Description("Anchored position X")] public float? PosX { get; set; }
        [Description("Anchored position Y")] public float? PosY { get; set; }
    }
}
