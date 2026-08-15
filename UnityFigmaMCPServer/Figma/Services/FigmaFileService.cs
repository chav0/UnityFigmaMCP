using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Server.Figma
{
    internal sealed class FigmaFileService : IFigmaFileService
    {
        private readonly IFigmaApiClient _client;

        public FigmaFileService(IFigmaApiClient client) => _client = client;

        public async Task<string> GetNodeAsync(string fileKey, string nodeId, CancellationToken ct)
        {
            var file = await _client.GetNodeAsync(fileKey, nodeId, ct).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(file, Formatting.Indented);

            var path = FigmaPathHelper.GetNodePath(fileKey, nodeId);
            await File.WriteAllTextAsync(path, json, ct).ConfigureAwait(false);

            return path;
        }

        public string GetNodeNames(string fileKey, string nodeId)
        {
            var path = FigmaPathHelper.GetNodePath(fileKey, nodeId);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Node '{nodeId}' for key '{fileKey}' not found. Call figma_get_node first to download it.");

            var file = JsonConvert.DeserializeObject<FigmaFile>(File.ReadAllText(path));

            var node = file?.root.FindNode(nodeId);
            if (node == null)
                throw new InvalidOperationException($"Node '{nodeId}' not found in the saved file for key '{fileKey}'.");

            return JsonConvert.SerializeObject(node.ToSlim());
        }

        public string GetComponentInfo(string fileKey, string rootNodeId, string targetNodeId)
        {
            var path = FigmaPathHelper.GetNodePath(fileKey, rootNodeId);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Node '{rootNodeId}' for key '{fileKey}' not found. Call figma_get_node first.");

            var file = JsonConvert.DeserializeObject<FigmaFile>(File.ReadAllText(path))
                       ?? throw new InvalidOperationException($"Failed to deserialize file for key '{fileKey}'.");

            var node = file.root.FindNode(targetNodeId)
                       ?? throw new InvalidOperationException($"Node '{targetNodeId}' not found in the subtree of '{rootNodeId}'.");

            string? componentKey = null;
            string? componentSetKey = null;
            string? componentSetId = null;

            if (node.type == FigmaObjectType.COMPONENT)
            {
                componentKey = file.GetComponentKey(node.id) ?? node.id;
            }
            else if (node.type == FigmaObjectType.INSTANCE && !string.IsNullOrEmpty(node.componentId))
            {
                componentKey = file.GetComponentKey(node.componentId) ?? node.componentId;
                componentSetId = file.GetComponentSetId(node.componentId);
                if (!string.IsNullOrEmpty(componentSetId))
                    componentSetKey = file.GetComponentKey(componentSetId) ?? componentSetId;
            }

            return JsonConvert.SerializeObject(new
            {
                nodeId = node.id,
                name = node.name,
                type = node.type.ToString(),
                componentId = node.componentId,
                componentKey,
                componentSetId,
                componentSetKey
            });
        }

        public Dictionary<string, ResolvedNodeInfo> ResolveNodeInfo(string fileKey, string[] nodeIds)
        {
            var result = new Dictionary<string, ResolvedNodeInfo>(nodeIds.Length);
            var dir = FigmaPathHelper.GetFileDir(fileKey);

            if (!Directory.Exists(dir))
            {
                foreach (var id in nodeIds)
                    result[id] = new ResolvedNodeInfo { ComponentKey = id };
                return result;
            }

            var files = new List<FigmaFile>();
            foreach (var jsonFile in Directory.GetFiles(dir, "*.json"))
            {
                var file = JsonConvert.DeserializeObject<FigmaFile>(File.ReadAllText(jsonFile));
                if (file != null)
                    files.Add(file);
            }

            foreach (var nodeId in nodeIds)
            {
                result[nodeId] = ResolveNode(files, nodeId);
            }

            return result;
        }

        private static ResolvedNodeInfo ResolveNode(List<FigmaFile> files, string nodeId)
        {
            foreach (var file in files)
            {
                var node = file.root?.FindNode(nodeId);
                if (node == null)
                    continue;

                var info = new ResolvedNodeInfo
                {
                    ComponentKey = nodeId,
                    FigmaName = node.name
                };

                var componentNodeId = node.type switch
                {
                    FigmaObjectType.INSTANCE when !string.IsNullOrEmpty(node.componentId) => node.componentId,
                    FigmaObjectType.COMPONENT => node.id,
                    _ => null
                };

                if (componentNodeId != null)
                {
                    var key = file.GetComponentKey(componentNodeId);
                    if (!string.IsNullOrEmpty(key))
                        info.ComponentKey = key;
                }

                return info;
            }

            return new ResolvedNodeInfo { ComponentKey = nodeId };
        }
    }
}
