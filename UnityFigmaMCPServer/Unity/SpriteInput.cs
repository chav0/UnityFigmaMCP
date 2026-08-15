using System.ComponentModel;

namespace UnityFigmaMCP.Server.Unity
{
    public class SpriteInput
    {
        [Description("Figma node ID to export (e.g. \"123:456\")")]
        public string NodeId { get; set; }

        [Description("Name for the sprite asset")]
        public string SpriteName { get; set; }
    }
}
