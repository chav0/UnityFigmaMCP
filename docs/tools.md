# MCP Tools Reference

UnityFigmaMCP exposes 13 tools via the Model Context Protocol.

## Status

### `status`

Check whether the server is ready: Figma token validity, Unity Editor connection, protocol version.

Call this before starting a workflow, or first when any tool fails — it tells you which half is broken.

**Parameters:** none

## Figma Tools

### `figma_get_node`

Fetch a specific node (frame, component, component set) from a Figma file and save it locally.
This is always the first step — other tools operate on the saved data.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `fileKey` | string | yes | Figma file key from the URL |
| `nodeId` | string | yes | Node ID, e.g. `"123:456"` |

**Returns:** absolute path to the saved JSON file.

### `figma_get_node_names`

Get a slim tree (id, name, type, children) of a previously fetched node. No network call — reads
the local copy. Use for quick orientation in the hierarchy.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `fileKey` | string | yes | Figma file key |
| `nodeId` | string | yes | Node ID |

### `figma_get_component_info`

Get the Figma component key and component set info for a node within a previously fetched subtree.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `fileKey` | string | yes | Figma file key |
| `rootNodeId` | string | yes | Root node ID that was fetched via `figma_get_node` |
| `targetNodeId` | string | yes | Target node ID to get component info for |

### `figma_export_image`

Export one or more Figma nodes as PNG images. Batches multiple node IDs into a single API call.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `fileKey` | string | yes | Figma file key |
| `nodeIds` | string[] | yes | Node IDs to export |
| `scale` | float | no | Export scale (default: `1`) |

**Returns:** JSON mapping each node ID to its saved file path.

## Build Tools

### `unity_get_pipelines`

List available layout pipeline profiles configured in Unity. Each pipeline defines
a set of steps applied when building prefabs.

**Parameters:** none

**Returns:** list of pipelines with IDs and step names.

### `unity_build_prefab`

Build a Unity UI prefab from a Figma node previously fetched via `figma_get_node`.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `fileKey` | string | yes | Figma file key |
| `nodeId` | string | yes | Figma node ID to build from |
| `prefabName` | string | yes | Name for the prefab asset |
| `savePath` | string | no | Asset folder path (default: from settings) |
| `pipelineId` | string | no | Pipeline profile ID (default: first available) |

When the node is a `COMPONENT_SET`, builds the base prefab plus variant prefabs automatically.

## Edit Tools

### `unity_edit_prefab`

Apply a batch of edits to one prefab: add, update or remove components and restructure
the hierarchy. All edits run in a single open/save cycle — failure leaves the prefab untouched.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `prefabPath` | string | yes | Prefab asset path |
| `edits` | PrefabEdit[] | yes | Edits to apply, in order |
| `includeChildren` | bool | no | Include child tree in response (default: `false`) |

Every edit carries an `op` and a `path` to the target object inside the prefab
(null or empty targets the root).

**Component ops** — each reads its matching payload field and adds the component
if it is missing. Set `remove: true` and omit the payload to strip it instead.

| Op | Payload field | Unity component |
|----|---------------|-----------------|
| `rect` | `rect` | RectTransform |
| `img` | `img` | Image |
| `text` | `text` | TextMeshProUGUI |
| `hLayout` | `hLayout` | HorizontalLayoutGroup |
| `vLayout` | `vLayout` | VerticalLayoutGroup |
| `grid` | `grid` | GridLayoutGroup |
| `fitter` | `fitter` | ContentSizeFitter |

**Hierarchy ops**

| Op | Required | Description |
|----|----------|-------------|
| `create` | `name` | Create an empty GameObject under `path` |
| `instantiate` | `prefab` | Add an existing prefab as a nested instance under `path` |
| `delete` | — | Delete the object at `path` |
| `reparent` | `newParentPath` | Move the object; `siblingIndex` optional |
| `setActive` | `active` | Toggle the object's active state |

List `create` and `instantiate` first so later edits can reference the paths they
produce, and `delete` last so it does not invalidate paths still in use.

```json
[
  { "op": "create", "path": "Content", "name": "Badge" },
  { "op": "rect", "path": "Content/Badge", "rect": { "size": [24, 24] } },
  { "op": "instantiate", "path": "Content", "prefab": "Assets/UI/Prefabs/Card.prefab" },
  { "op": "text", "path": "Header/Title", "text": { "text": "Updated", "size": 18 } }
]
```

### `unity_save_prefab`

Extract a subtree from an existing prefab and save it as a separate prefab asset.
The subtree becomes a nested prefab instance in the source.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `prefabPath` | string | yes | Source prefab asset path |
| `objectPath` | string | no | Relative path to the child object (null = root) |
| `assetPath` | string | no | Where to save the new prefab |
| `componentKey` | string | no | Figma component key to register |

### `unity_get_hierarchy`

Inspect the GameObject hierarchy of a prefab with all components and properties.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `prefabPath` | string | yes | Prefab asset path |
| `objectPath` | string | no | Child path for subtree (null = full hierarchy) |

Nested prefab instances are returned as leaf nodes: they carry `GO.Prefab` with the
asset path but no `Children`, so a deep tree does not inline every nested asset.
To look inside one, call `unity_get_hierarchy` again on that asset path.

## Asset Tools

### `unity_save_sprites`

Export Figma nodes as PNGs and import them into Unity as sprites in a single batch.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `fileKey` | string | yes | Figma file key |
| `sprites` | SpriteInput[] | yes | `{ nodeId, spriteName }` pairs |
| `scale` | float | no | Export scale (default: `1`) |
| `savePath` | string | no | Asset folder for sprites |

Automatically resolves Figma component keys for sprite map registration.

### `unity_bind`

Bind existing Unity assets to Figma keys so later builds reuse them instead of regenerating.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `kind` | string | yes | `"prefab"`, `"sprite"` or `"variant"` |
| `fileKey` | string | yes | Figma file key |
| `assets` | BindInput[] | yes | `{ assetPath, nodeId }` pairs |

Component keys and Figma names are resolved automatically from cached node data,
so call `figma_get_node` on the relevant nodes first.

Use `"variant"` to bind a prefab as a variant of its Figma component set. The parent
set is resolved from the node, and the variant is stored under its Figma name
(`Selection=Chosen`), so several variants may point at the same prefab.

### `unity_list_assets`

List the prefab or sprite assets in the project's configured folder with their Figma bindings.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `kind` | string | yes | `"prefab"` or `"sprite"` |
| `query` | string | no | Name filter — `"Card"` matches `CardSmall` and `IconCard` (null lists everything) |

Each entry shows the asset name, path, and `figmaKey` (null if unbound).
