using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class ImageComponent
    {
        [Description("Sprite name to resolve through the sprite map. Ignored when SpritePath is set.")]
        public string SpriteName { get; set; }

        [Description("Sprite asset path (e.g. \"Assets/UI/Sprites/icon.png\")")]
        public string SpritePath { get; set; }

        [Description("Hex color (e.g. \"#FFFFFFFF\")")]
        public string Color { get; set; }

        [Description("Image type: \"simple\", \"sliced\", \"tiled\" or \"filled\"")]
        public string Type { get; set; }

        [Description("Whether the image blocks raycasts")]
        public bool? RaycastTarget { get; set; }
    }
}
