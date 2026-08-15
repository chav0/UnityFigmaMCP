using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFigmaMCP.Server.Figma
{
    internal sealed class FigmaImageService : IFigmaImageService
    {
        private readonly IFigmaApiClient _client;

        public FigmaImageService(IFigmaApiClient client) => _client = client;

        public async Task<string> ExportImageAsync(string fileKey, string nodeId, float scale, CancellationToken ct)
        {
            var bytes = await _client.GetNodeImageAsync(fileKey, nodeId, scale, ct).ConfigureAwait(false);

            if (bytes == null || bytes.Length == 0)
                throw new InvalidOperationException("No image returned for node " + nodeId);

            var path = FigmaPathHelper.GetImagePath(fileKey, nodeId, scale);
            await File.WriteAllBytesAsync(path, bytes, ct).ConfigureAwait(false);

            return path;
        }

        public async Task<Dictionary<string, string>> ExportImagesAsync(string fileKey, string[] nodeIds, float scale, CancellationToken ct)
        {
            var imagesData = await _client.GetNodesImagesAsync(fileKey, nodeIds, scale, ct).ConfigureAwait(false);

            var paths = new Dictionary<string, string>(imagesData.Count);
            foreach (var entry in imagesData)
            {
                var path = FigmaPathHelper.GetImagePath(fileKey, entry.Key, scale);
                await File.WriteAllBytesAsync(path, entry.Value, ct).ConfigureAwait(false);
                paths[entry.Key] = path;
            }

            return paths;
        }
    }
}
