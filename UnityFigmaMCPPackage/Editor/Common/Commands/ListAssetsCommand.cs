namespace UnityFigmaMCP.Common
{
    public static class AssetKinds
    {
        public const string Prefab = "prefab";
        public const string Sprite = "sprite";
        public const string Variant = "variant";
    }

    public class ListAssetsCommand : ICommand<ListAssetsCommandResult>
    {
        public string Kind { get; set; }

        public string Query { get; set; }
    }
}
