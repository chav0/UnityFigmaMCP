using System.Threading;
using System.Threading.Tasks;

namespace UnityFigmaMCP.Server.Figma
{
    public interface IFigmaAuthService
    {
        Task<FigmaStatus> GetStatusAsync(CancellationToken ct);
    }
}
