namespace UnityFigmaMCP.Common
{
    public class ListAssetsCommandResult
    {
        public string Kind { get; set; }
        public string Folder { get; set; }
        public AssetInfo[] Assets { get; set; }
    }

    public class AssetInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string FigmaKey { get; set; }
        public AssetInfo[] Variants { get; set; }
    }
}
