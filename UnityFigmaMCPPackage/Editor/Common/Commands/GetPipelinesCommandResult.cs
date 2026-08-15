namespace UnityFigmaMCP.Common
{
    public class GetPipelinesCommandResult
    {
        public PipelineInfo[] Pipelines { get; set; }
    }

    public class PipelineInfo
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }
}
