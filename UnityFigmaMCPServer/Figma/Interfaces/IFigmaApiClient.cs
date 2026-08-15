using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Server.Figma
{
    public interface IFigmaApiClient
    {
        Task<FigmaFile> GetNodeAsync(string fileKey, string nodeId, CancellationToken ct = default);
        Task<byte[]?> GetNodeImageAsync(string fileKey, string nodeId, float scale = 1f, CancellationToken ct = default);
        Task<Dictionary<string, byte[]>> GetNodesImagesAsync(string fileKey, string[] nodeIds, float scale = 1f, CancellationToken ct = default);
        Task<FigmaUser> GetCurrentUserAsync(CancellationToken ct = default);
    }
}
