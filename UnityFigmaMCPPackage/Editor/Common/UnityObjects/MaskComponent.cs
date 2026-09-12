using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class MaskComponent
    {
        [Description("False to hide the mask's own graphic (omitted when true)")]
        public bool? Show;

        [Description("Use RectMask2D instead of Mask (better performance, no stencil)")]
        public bool IsRect;
    }
}
