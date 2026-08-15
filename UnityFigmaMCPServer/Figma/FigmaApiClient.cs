using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Server.Figma
{
    public sealed class FigmaApiClient : IFigmaApiClient, IDisposable
    {
        private const string BaseUrl = "https://api.figma.com/v1/";
        private const string TokenHeader = "X-Figma-Token";

        private readonly HttpClient _httpClient;

        public FigmaApiClient(string token)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            if (!string.IsNullOrWhiteSpace(token))
                _httpClient.DefaultRequestHeaders.Add(TokenHeader, token);
        }

        public async Task<FigmaFile> GetNodeAsync(string fileKey, string nodeId, CancellationToken ct = default)
        {
            var encodedIds = Uri.EscapeDataString(nodeId);
            var json = await GetStringAsync($"files/{fileKey}/nodes?ids={encodedIds}", ct).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<FigmaNodeTree>(json);

            if (response?.nodes == null
                || !response.nodes.TryGetValue(nodeId, out var nodeData)
                || nodeData?.document == null)
                throw new InvalidOperationException($"Node {nodeId} not found in file.");

            return new FigmaFile
            {
                name = response.name,
                components = nodeData.components,
                root = nodeData.document
            };
        }

        public async Task<byte[]?> GetNodeImageAsync(string fileKey, string nodeId, float scale = 1f, CancellationToken ct = default)
        {
            var encodedIds = Uri.EscapeDataString(nodeId);
            var scaleStr = scale.ToString(CultureInfo.InvariantCulture);

            var json = await GetStringAsync($"images/{fileKey}?ids={encodedIds}&format=png&scale={scaleStr}", ct).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<FigmaImageExport>(json);

            if (result?.images == null
                || !result.images.TryGetValue(nodeId, out var imageUrl)
                || string.IsNullOrEmpty(imageUrl))
                return null;

            return await GetBytesAsync(imageUrl, ct).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, byte[]>> GetNodesImagesAsync(string fileKey, string[] nodeIds, float scale = 1f, CancellationToken ct = default)
        {
            var encodedIds = Uri.EscapeDataString(string.Join(",", nodeIds));
            var scaleStr = scale.ToString(CultureInfo.InvariantCulture);

            var json = await GetStringAsync($"images/{fileKey}?ids={encodedIds}&format=png&scale={scaleStr}", ct).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<FigmaImageExport>(json);

            var images = new Dictionary<string, byte[]>();
            if (result?.images == null)
                return images;

            foreach (var entry in result.images)
            {
                ct.ThrowIfCancellationRequested();
                if (string.IsNullOrEmpty(entry.Value))
                    continue;
                var bytes = await GetBytesAsync(entry.Value, ct).ConfigureAwait(false);
                images[entry.Key] = bytes;
            }

            return images;
        }

        public async Task<FigmaUser> GetCurrentUserAsync(CancellationToken ct = default)
        {
            var json = await GetStringAsync("me", ct).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<FigmaUser>(json);
        }

        private async Task<string> GetStringAsync(string url, CancellationToken ct)
        {
            using var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private async Task<byte[]> GetBytesAsync(string url, CancellationToken ct)
        {
            using var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}
