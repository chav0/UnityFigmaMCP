using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using Newtonsoft.Json;

namespace UnityFigmaMCP.Server.Figma
{
    [McpServerToolType]
    public sealed class FigmaMcpServerTools
    {
        private readonly IFigmaFileService _files;
        private readonly IFigmaImageService _images;

        public FigmaMcpServerTools(IFigmaFileService files, IFigmaImageService images)
        {
            _files = files;
            _images = images;
        }

        [McpServerTool(Name = "figma_get_node")]
        [Description("Fetch a specific node (frame, component, etc.) from a Figma file by its node ID and save it to a temp JSON file. Returns the absolute path of the saved file.")]
        public Task<string> GetNode(
            [Description("Figma file key")] string fileKey,
            [Description("Node ID (e.g. \"123:456\")")] string nodeId,
            CancellationToken ct = default)
            => _files.GetNodeAsync(fileKey, nodeId, ct);

        [McpServerTool(Name = "figma_get_node_names")]
        [Description("Get a slim tree (id, name, type, children only) of a node previously fetched via figma_get_node, or of any node inside one. " +
                     "Reads the saved working copy — no network call, so call figma_get_node first and do not run the two in parallel. " +
                     "Pass depth to keep the answer small: nodes cut off at that depth show childCount instead of children — " +
                     "call again with one of their ids to look deeper.")]
        public string GetNodeNames(
            [Description("Figma file key")] string fileKey,
            [Description("Node ID (e.g. \"123:456\") — the fetched node or any node inside it")] string nodeId,
            [Description("How many levels below the node to include (e.g. 3). 0 returns only the node itself. Null returns the whole tree.")] int? depth = null)
            => _files.GetNodeNames(fileKey, nodeId, depth);

        [McpServerTool(Name = "figma_get_component_info")]
        [Description("Get the Figma component key and component set info for a node within a previously fetched subtree. Use to discover the figmaKey needed for unity_bind.")]
        public string GetComponentInfo(
            [Description("Figma file key")] string fileKey,
            [Description("Root node ID that was fetched via figma_get_node")] string rootNodeId,
            [Description("Target node ID to get component info for")] string targetNodeId)
            => _files.GetComponentInfo(fileKey, rootNodeId, targetNodeId);

        [McpServerTool(Name = "figma_export_image")]
        [Description("Export one or more Figma nodes as PNG images and save them to temp files. Pass multiple node IDs to batch into a single Figma API call. Returns a JSON object mapping each node ID to its saved file path.")]
        public async Task<string> ExportImage(
            [Description("Node IDs to export (e.g. [\"123:456\"] or [\"123:456\", \"789:012\"])")] string[] nodeIds,
            [Description("Figma file key")] string fileKey,
            [Description("Export scale. MUST be 1. Do NOT change this value unless the user explicitly requests a different scale.")] float scale = 1f,
            CancellationToken ct = default)
        {
            var paths = await _images.ExportImagesAsync(fileKey, nodeIds, scale, ct).ConfigureAwait(false);
            return JsonConvert.SerializeObject(paths);
        }
    }
}
