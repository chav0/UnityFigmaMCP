namespace UnityFigmaMCP.Common
{
    public class EditPrefabCommand : ICommand<EditPrefabCommandResult>
    {
        public string PrefabPath { get; set; }
        public PrefabEdit[] Edits { get; set; }
        public bool IncludeChildren { get; set; }
    }
}
