using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using UnityFigmaMCP.Common;
using UnityFigmaMCP.Server.Figma;

namespace UnityFigmaMCP.Server.Unity
{
    [McpServerToolType]
    public sealed class UnityMcpServerTools
    {
        private readonly IUnityCommandService _commandService;
        private readonly IFigmaImageService _figmaImages;
        private readonly IFigmaFileService _figmaFiles;

        public UnityMcpServerTools(IUnityCommandService commandService, IFigmaImageService figmaImages, IFigmaFileService figmaFiles)
        {
            _commandService = commandService;
            _figmaImages = figmaImages;
            _figmaFiles = figmaFiles;
        }

        [McpServerTool(Name = "unity_get_pipelines")]
        [Description("Get the list of available layout pipeline profiles configured in Unity. " +
                     "Each pipeline defines a set of steps (Text, RectTransform, Image, Layout, etc.) applied when building prefabs. " +
                     "Use this to discover valid pipelineId values for unity_build_prefab.")]
        public Task<string> GetPipelines(CancellationToken ct = default)
            => _commandService.SendCommandAsync(new GetPipelinesCommand(), ct);

        [McpServerTool(Name = "unity_build_prefab")]
        [Description("Build a Unity UI prefab from a Figma node previously fetched via figma_get_node. " +
                     "Runs the selected layout pipeline in Unity Editor and saves the result as a prefab asset. " +
                     "Call unity_get_pipelines first to discover available pipeline IDs. " +
                     "If savePath is null, uses the default prefab folder from FigmaAutoLayoutSettings. " +
                     "When the node is a COMPONENT_SET, builds the base prefab plus variant prefabs — " +
                     "the response includes a 'variants' array with each variant's name, path, and figmaKey. " +
                     "Variant prefabs start as copies of the base and must be edited individually via unity_edit_prefab to match their Figma variant.")]
        public Task<string> BuildPrefab(
            [Description("Figma file key")] string fileKey,
            [Description("Figma node ID to build the prefab from (e.g. \"123:456\")")] string nodeId,
            [Description("Name for the prefab asset")] string prefabName,
            [Description("Asset folder path to save the prefab into (e.g. \"Assets/UI/Prefabs\"). Null uses the default from settings.")] string? savePath = null,
            [Description("Pipeline profile ID (get available IDs via unity_get_pipelines). Null uses the first available pipeline.")] string? pipelineId = null,
            CancellationToken ct = default)
        {
            var nodeJsonPath = FigmaPathHelper.GetNodePath(fileKey, nodeId);
            if (!File.Exists(nodeJsonPath))
                throw new FileNotFoundException($"Node '{nodeId}' for key '{fileKey}' is not downloaded. Call figma_get_node first.");

            return _commandService.SendCommandAsync(new BuildPrefabCommand
            {
                PrefabName = prefabName,
                SavePath = savePath,
                NodeJsonPath = nodeJsonPath,
                PipelineId = pipelineId
            }, ct);
        }

        [McpServerTool(Name = "unity_edit_prefab")]
        [Description("Apply a batch of edits to one prefab: add, update or remove components (RectTransform, Image, Text, layout groups, ContentSizeFitter) " +
                     "and restructure the hierarchy (create, delete, reparent, setActive). " +
                     "Every edit runs inside a single prefab open/save cycle in the given order, " +
                     "and the asset is saved only if all of them succeed — a failure leaves the prefab untouched. " +
                     "Batch related changes into one call instead of calling repeatedly. " +
                     "Call unity_get_hierarchy first to learn the object paths.")]
        public Task<string> EditPrefab(
            [Description("Prefab asset path (e.g. \"Assets/UI/Prefabs/Panel.prefab\")")] string prefabPath,
            [Description("Edits to apply, in order.")] PrefabEdit[] edits,
            [Description("Include the full child tree in the returned hierarchy. Default: false, which returns the root only.")] bool includeChildren = false,
            CancellationToken ct = default)
            => _commandService.SendCommandAsync(new EditPrefabCommand
            {
                PrefabPath = prefabPath,
                Edits = edits,
                IncludeChildren = includeChildren
            }, ct);

        [McpServerTool(Name = "unity_save_prefab")]
        [Description("Extract a subtree from an existing prefab and save it as a separate prefab asset. " +
                     "The subtree becomes a nested prefab instance in the source. " +
                     "Optionally registers the new prefab in the component map.")]
        public Task<string> SavePrefab(
            [Description("Prefab asset path (e.g. \"Assets/UI/Prefabs/Panel.prefab\")")] string prefabPath,
            [Description("Relative path to the child object inside the prefab (e.g. \"Content/Card\"). Null for the root.")] string? objectPath = null,
            [Description("Asset path to save the new prefab to (e.g. \"Assets/UI/Prefabs/Card.prefab\")")] string? assetPath = null,
            [Description("Figma component key to register in the component map. Optional.")] string? componentKey = null,
            CancellationToken ct = default)
            => _commandService.SendCommandAsync(new SavePrefabCommand
            {
                PrefabPath = prefabPath,
                ObjectPath = objectPath,
                AssetPath = assetPath,
                ComponentKey = componentKey
            }, ct);

        [McpServerTool(Name = "unity_get_hierarchy")]
        [Description("Inspect the GameObject hierarchy of a prefab. " +
                     "Returns the full tree for the prefab root, or a subtree rooted at the specified child.")]
        public Task<string> GetHierarchy(
            [Description("Prefab asset path (e.g. \"Assets/UI/Prefabs/Panel.prefab\")")] string prefabPath,
            [Description("Relative path to a child object inside the prefab (e.g. \"Header/Title\"). Null returns the full hierarchy.")] string? objectPath = null,
            CancellationToken ct = default)
            => _commandService.SendCommandAsync(new GetHierarchyCommand
            {
                PrefabPath = prefabPath,
                ObjectPath = objectPath
            }, ct);

        [McpServerTool(Name = "unity_save_sprites")]
        [Description("Export Figma nodes as PNGs and import them into Unity as sprite assets in a single batch. " +
                     "Uses one Figma API call and batches Unity asset imports. " +
                     "Automatically resolves Figma component keys from cached node data for correct sprite map registration.")]
        public async Task<string> SaveSprites(
            [Description("Figma file key")] string fileKey,
            [Description("Sprites to export and import")] SpriteInput[] sprites,
            [Description("Export scale. MUST be 1. Do NOT change this value unless the user explicitly requests a different scale.")] float scale = 1f,
            [Description("Asset folder path to save sprites into (e.g. \"Assets/UI/Sprites\"). Optional.")] string? savePath = null,
            CancellationToken ct = default)
        {
            var nodeIds = sprites.Select(s => s.NodeId).ToArray();
            var paths = await _figmaImages.ExportImagesAsync(fileKey, nodeIds, scale, ct).ConfigureAwait(false);
            var nodeInfo = _figmaFiles.ResolveNodeInfo(fileKey, nodeIds);

            var entries = sprites
                .Where(s => paths.ContainsKey(s.NodeId))
                .Select(s => new SpriteEntry
                {
                    SpritePath = paths[s.NodeId],
                    SpriteName = s.SpriteName,
                    Id = nodeInfo[s.NodeId].ComponentKey,
                    FigmaName = nodeInfo[s.NodeId].FigmaName
                })
                .ToArray();

            return await _commandService.SendCommandAsync(new SaveSpritesCommand
            {
                Sprites = entries,
                SavePath = savePath
            }, ct).ConfigureAwait(false);
        }

        [McpServerTool(Name = "unity_bind")]
        [Description("Bind existing Unity assets to Figma keys so later builds reuse them instead of regenerating. " +
                     "Automatically resolves Figma component keys and names from cached node data (call figma_get_node first). " +
                     "Prefabs go into the component map; sprites go into the sprite map with both the component key and original Figma name for lookup. " +
                     "Use unity_list_assets to find assets whose figmaKey is still null.")]
        public Task<string> Bind(
            [Description("What is being bound: \"prefab\" or \"sprite\"")] string kind,
            [Description("Figma file key")] string fileKey,
            [Description("Assets to bind")] BindInput[] assets,
            CancellationToken ct = default)
        {
            var nodeIds = assets.Select(a => a.NodeId).ToArray();
            var nodeInfo = _figmaFiles.ResolveNodeInfo(fileKey, nodeIds);

            return _commandService.SendCommandAsync(new BindAssetCommand
            {
                Kind = kind,
                Assets = assets.Select(a => new BindEntry
                {
                    AssetPath = a.AssetPath,
                    FigmaKey = nodeInfo[a.NodeId].ComponentKey,
                    FigmaName = nodeInfo[a.NodeId].FigmaName
                }).ToArray()
            }, ct);
        }

        [McpServerTool(Name = "unity_list_assets")]
        [Description("List the prefab or sprite assets in the project's configured folder. " +
                     "Each entry has the asset name, its path, and the Figma key it is bound to (null when unbound) — " +
                     "use it to see what already exists before building or downloading anything.")]
        public Task<string> ListAssets(
            [Description("Which assets to list: \"prefab\" or \"sprite\"")] string kind,
            CancellationToken ct = default)
            => _commandService.SendCommandAsync(new ListAssetsCommand { Kind = kind }, ct);
    }
}
