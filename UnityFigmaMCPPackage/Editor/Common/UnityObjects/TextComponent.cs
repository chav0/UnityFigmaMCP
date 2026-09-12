using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class TextComponent
    {
        [Description("Text content")]
        public string Text { get; set; }

        [Description("Font size in points")]
        public float? Size { get; set; }

        [Description("Font family name (e.g. \"Roboto\", \"Inter\")")]
        public string Font { get; set; }

        [Description("Font style: \"Regular\", \"Bold\", \"Italic\" or \"BoldItalic\"")]
        public string Style { get; set; }

        [Description("Hex color (e.g. \"#000000FF\")")]
        public string Color { get; set; }

        [Description("Text alignment (e.g. \"MidlineLeft\", \"Center\", \"TopRight\")")]
        public string Align { get; set; }

        [Description("Whether the font size auto-fits the rect")]
        public bool? Auto { get; set; }
    }
}
