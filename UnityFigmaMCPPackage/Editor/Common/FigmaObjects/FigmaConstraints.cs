namespace UnityFigmaMCP.Common
{
    public class FigmaConstraints
    {
        public FigmaSeparatedConstraints vertical;
        public FigmaSeparatedConstraints horizontal;
    }

    public enum FigmaSeparatedConstraints
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        CENTER,
        TOP_BOTTOM,
        LEFT_RIGHT,
        SCALE
    }
}
