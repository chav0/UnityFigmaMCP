using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class GetHierarchyCommandHandler : ICommandHandler<GetHierarchyCommand, GetHierarchyCommandResult>
    {
        public GetHierarchyCommandResult Handle(ICommandContext context, GetHierarchyCommand command)
        {
            using var scope = PrefabEditScope.Create(command.PrefabPath, command.ObjectPath);

            return new GetHierarchyCommandResult
            {
                Root = UnityObjectConverter.Convert(scope.Target, context.Mappers.All, command.ObjectPath)
            };
        }
    }
}
