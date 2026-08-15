namespace UnityFigmaMCP.Common
{
    public class BindAssetCommand : ICommand<BindAssetCommandResult>
    {
        public string Kind { get; set; }
        public BindEntry[] Assets { get; set; }
    }

    public class BindEntry
    {
        public string AssetPath { get; set; }
        public string FigmaKey { get; set; }
        public string FigmaName { get; set; }
    }
}
