namespace UnityFigmaMCP.Common
{
    public class BuildPrefabCommandResult
    {
        public string AssetPath { get; set; }
        public UnityObject Root { get; set; }
        public AssetInfo[] Variants { get; set; }
    }
}
