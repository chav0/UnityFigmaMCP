namespace UnityFigmaMCP.Common
{
    public class FigmaFill
    {
        public FigmaBlendMode blendMode;
        public string type;
        public FigmaColor color;
        public string imageRef;
        public float? opacity; 
        public bool? visible; 
    }

    public enum FigmaBlendMode
    {
        NORMAL,
        DARKEN,
        MULTIPLY,
        COLOR_BURN,
        LIGHTEN, 
        SCREEN,
        COLOR_DODGE,
        OVERLAY,
        SOFT_LIGHT,
        HARD_LIGHT,
        DIFFERENCE,
        EXCLUSION,
        HUE,
        SATURATION,
        COLOR,
        LUMINOSITY,
        PASS_THROUGH
    }

    public class FigmaColor
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }
}