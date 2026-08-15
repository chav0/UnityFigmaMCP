using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal interface ICommandHandler<in TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        TResult Handle(ICommandContext context, TCommand command);
    }
}
