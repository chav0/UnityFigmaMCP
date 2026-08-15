using System.Linq;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class GetPipelinesCommandHandler : ICommandHandler<GetPipelinesCommand, GetPipelinesCommandResult>
    {
        public GetPipelinesCommandResult Handle(ICommandContext context, GetPipelinesCommand command)
        {
            var settings = context.LayoutSettings;

            var pipelines = settings.Pipelines
                .Select(pipeline => new PipelineInfo { Id = pipeline.Id, Description = pipeline.Description })
                .ToArray();

            return new GetPipelinesCommandResult { Pipelines = pipelines };
        }
    }
}
