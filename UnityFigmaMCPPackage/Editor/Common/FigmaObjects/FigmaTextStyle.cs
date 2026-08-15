namespace UnityFigmaMCP.Common
{
    public class FigmaTextStyle
    {
        public string fontFamily;
        public string fontPostScriptName;
        public float fontWeight;
        public float fontSize;
        public FigmaAlign textAlignHorizontal;
        public FigmaAlign textAlignVertical;
        public float letterSpacing;
        public float lineHeightPx;
        public float lineHeightPercent; 
        public float paragraphSpacing;
        public TextCase textCase; 
    }

    public enum TextCase
    {
        NONE,
        UPPER,
        LOWER,
        TITLE
    }
}
