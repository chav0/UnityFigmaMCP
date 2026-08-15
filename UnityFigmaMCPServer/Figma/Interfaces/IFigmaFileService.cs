using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFigmaMCP.Server.Figma
{
    public interface IFigmaFileService
    {
        Task<string> GetNodeAsync(string fileKey, string nodeId, CancellationToken ct);
        string GetNodeNames(string fileKey, string nodeId);
        string GetComponentInfo(string fileKey, string rootNodeId, string targetNodeId);
        Dictionary<string, ResolvedNodeInfo> ResolveNodeInfo(string fileKey, string[] nodeIds);
    }
}
