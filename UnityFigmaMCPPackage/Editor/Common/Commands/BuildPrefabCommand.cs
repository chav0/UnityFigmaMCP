namespace UnityFigmaMCP.Common
{
    public class BuildPrefabCommand : ICommand<BuildPrefabCommandResult>
    {
        public string PrefabName { get; set; }
        public string SavePath { get; set; }
        public string NodeJsonPath { get; set; }
        public string PipelineId { get; set; }
    }
}
