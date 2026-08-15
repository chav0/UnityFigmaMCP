namespace UnityFigmaMCP.Common
{
    public class GetHierarchyCommand : ICommand<GetHierarchyCommandResult>
    {
        public string PrefabPath { get; set; }
        public string ObjectPath { get; set; }
    }
}
