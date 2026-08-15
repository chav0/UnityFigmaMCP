using System.Threading;
using System.Threading.Tasks;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Server.Unity
{
    public interface IUnityCommandService
    {
        Task<string> SendCommandAsync<TCommand>(TCommand command, CancellationToken ct) where TCommand : ICommand;
    }
}
