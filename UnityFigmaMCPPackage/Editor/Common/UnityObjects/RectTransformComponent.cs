using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class RectTransformComponent
    {
        [Description("Anchors [minX, minY, maxX, maxY], range 0..1 (e.g. [0, 0, 1, 1] for stretch)")]
        public float[] Anchors { get; set; }

        [Description("Pivot [x, y], range 0..1 (e.g. [0.5, 0.5])")]
        public float[] Pivot { get; set; }

        [Description("Size [width, height] in pixels")]
        public float[] Size { get; set; }

        [Description("Anchored position [x, y] in pixels")]
        public float[] Pos { get; set; }
    }
}
