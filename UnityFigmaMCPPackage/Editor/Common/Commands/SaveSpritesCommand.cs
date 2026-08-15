namespace UnityFigmaMCP.Common
{
    public class SaveSpritesCommand : ICommand<SaveSpritesCommandResult>
    {
        public SpriteEntry[] Sprites { get; set; }
        public string SavePath { get; set; }
    }

    public class SpriteEntry
    {
        public string SpritePath { get; set; }
        public string SpriteName { get; set; }
        public string Id { get; set; }
        public string FigmaName { get; set; }
    }
}
