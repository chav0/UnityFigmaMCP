using System.ComponentModel;

namespace UnityFigmaMCP.Server.Unity
{
    public class BindInput
    {
        [Description("Asset path (e.g. \"Assets/UI/Prefabs/Button.prefab\" or \"Assets/UI/Sprites/icon.png\")")]
        public string AssetPath { get; set; }

        [Description("Figma node ID (e.g. \"123:456\")")]
        public string NodeId { get; set; }
    }
}
