namespace UnityFigmaMCP.Common
{
    public static class AssetKinds
    {
        public const string Prefab = "prefab";
        public const string Sprite = "sprite";
    }

    public class ListAssetsCommand : ICommand<ListAssetsCommandResult>
    {
        public string Kind { get; set; }
    }
}
