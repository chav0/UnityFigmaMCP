using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFigmaMCP.Server.Figma
{
    public interface IFigmaImageService
    {
        Task<string> ExportImageAsync(string fileKey, string nodeId, float scale, CancellationToken ct);
        Task<Dictionary<string, string>> ExportImagesAsync(string fileKey, string[] nodeIds, float scale, CancellationToken ct);
    }
}
