namespace UnityFigmaMCP.Common
{
    public class SaveSpritesCommandResult
    {
        public SpriteResult[] Sprites { get; set; }
    }

    public class SpriteResult
    {
        public string AssetPath { get; set; }
        public string SpriteName { get; set; }
    }
}
