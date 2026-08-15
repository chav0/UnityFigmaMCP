namespace UnityFigmaMCP.Common
{
    public class SavePrefabCommand : ICommand<SavePrefabCommandResult>
    {
        public string PrefabPath { get; set; }
        public string ObjectPath { get; set; }
        public string AssetPath { get; set; }
        public string ComponentKey { get; set; }
    }
}
