---
name: bind-figma-unity
description: >
  Bind existing Unity prefabs and sprites to Figma components — only when the user
  explicitly asks. Binding is NOT a prerequisite for building or updating prefabs.
  Use this when an old UI kit already has assets that are not linked to Figma components.
  Trigger phrases: "bind figma", "sync figma", "match components", "map ui kit",
  "link figma to unity". Do NOT run this skill just because the user asked to build,
  layout, or update a prefab.
---

# Bind Figma–Unity — Component & Sprite Matching

Binding means linking Figma objects to Unity assets that already exist. It does **not**
download, create, or rebuild anything — it only writes entries in the component map
and sprite map so that `unity_build_prefab` can reuse them instead of rebuilding subtrees.

## When to use (opt-in only)

**Do not run this skill unless the user asks to bind, map, or link assets.**

It is **not** a step before `/build-prefab`. The pipeline already registers keys
when you download sprites and build prefabs.

Use it only when:

- The user explicitly asks to bind / map / link Figma to existing Unity assets
- You are working with an **old UI kit**: prefabs and sprites already live in the
  project, but they were made without Figma keys and are not connected to Figma
  components

Do **not** re-bind assets that already have a `figmaKey`.

## Input

The user needs to provide:
- **Figma file key** — the key from the Figma URL
- **Node ID** — the root node to scan (a page, frame, or component set)

If the user gives a Figma URL, extract the file key and node ID from it.

## Workflow

### Step 1 — Fetch the Figma tree

Call `figma_get_node` with the file key and node ID to download the full subtree.
Then call `figma_get_node_names` to get the slim hierarchy for analysis.

### Step 2 — Get existing Unity assets

Call `unity_list_assets` twice in parallel:
- `kind: "prefab"` — all prefabs with their names, paths, and existing bindings
- `kind: "sprite"` — all sprites with their names, paths, and existing bindings

Each entry carries a `figmaKey` — null means the asset is not bound yet.

### Step 3 — Match by name

Walk the Figma node tree and collect components, component sets, and image/icon nodes.
For each, try to find a matching unbound Unity asset by name:
- Normalize both names (lowercase, strip spaces/hyphens/underscores) for comparison
- A Figma component "Button / Primary" should match a prefab "ButtonPrimary",
  "button-primary", "Button_Primary", etc.
- Skip assets that already have a `figmaKey`

Build two lists of matches: `{ assetPath, nodeId }` pairs for prefabs and sprites.

### Step 4 — Register bindings

Call `unity_bind` for each kind with matched assets:

```
unity_bind(kind: "prefab", fileKey: "<key>", assets: [
  { assetPath: "Assets/UI/Prefabs/Button.prefab", nodeId: "123:456" },
  { assetPath: "Assets/UI/Prefabs/Card.prefab", nodeId: "124:789" }
])

unity_bind(kind: "sprite", fileKey: "<key>", assets: [
  { assetPath: "Assets/UI/Sprites/icon-search.png", nodeId: "125:100" },
  ...
])
```

The server resolves component keys and Figma names automatically from cached node
data — you only need to pass asset paths and node IDs.

### Step 5 — Report

Present results in the user's language:

```
## Figma–Unity Binding Report

Source: figma.com/file/<key>, node 123:456

### Components
Bound: 8 new bindings
Already bound: 3 (skipped)

| Figma Component     | Unity Prefab                          | Status       |
|---------------------|---------------------------------------|--------------|
| Button / Primary    | Assets/UI/Prefabs/ButtonPrimary.prefab | Bound        |
| Card                | Assets/UI/Prefabs/Card.prefab         | Bound        |
| Avatar              | —                                     | Not found    |
| Header              | Assets/UI/Prefabs/Header.prefab       | Already bound |

### Sprites
Bound: 15 new bindings
Already bound: 10 (skipped)

| Figma Node          | Unity Sprite                           | Status       |
|---------------------|----------------------------------------|--------------|
| icon-search         | Assets/UI/Sprites/icon-search.png      | Bound        |
| icon-bell           | —                                      | Not found    |

### Missing in Unity
These Figma assets have no match in the project:
- Components: Avatar, TabBar, BottomSheet
- Sprites: icon-bell, icon-camera, bg-gradient

Use `/download-sprites` to import missing sprites.
Build missing components with `/build-prefab`.
```

## Incremental sync

This skill supports re-running. On subsequent runs it skips already-bound assets
and only processes new or unmatched ones.
