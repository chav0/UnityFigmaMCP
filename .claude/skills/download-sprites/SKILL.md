---
name: download-sprites
description: >
  Download sprites from Figma and import them into Unity as sprite assets. Use this skill
  whenever the user wants to export images or icons from Figma into their Unity project,
  save sprites, download assets, or import graphics. Trigger phrases: "download sprites",
  "export sprites", "save icons from figma", "import images".
---

# Download Sprites — Figma to Unity

This skill exports image nodes from Figma as PNGs and imports them into the Unity project
as sprite assets. Each sprite is automatically registered in the FigmaSpriteMap for use
during prefab building.

## When to use

The user wants to get specific images out of Figma and into Unity — icons, backgrounds,
illustrations, or any visual element that should become a sprite asset rather than being
built from primitives. This is a bulk-download workflow, not a single-image export.

## Prerequisites

Before starting, verify the basics are in place (don't run the full setup-project check —
just confirm the Figma token works and Unity is connected):

1. Call `status` — `figma.tokenValid` and `unity.editorConnected` must both be true
2. Call `unity_list_assets` with `kind: "sprite"` — shows what's already imported

If either fails, tell the user to run `/setup-project` first.

## Workflow

### Step 1 — Identify what to download

The user may provide sprites in different ways:

**A) Figma file key + node ID(s):**
The user gives specific node IDs. Fetch the node tree with `figma_get_node` and
`figma_get_node_names` to understand what they point at.

**B) Figma file key + parent node ID:**
The user points to a frame or page that contains many sprites. Fetch the tree,
show the node names to the user, and ask which ones to export — or export all
leaf nodes / component instances.

**C) A list of sprite names:**
The user has names but no IDs. They need to provide the Figma file key and a
parent node ID so you can search the tree for matching names.

### Step 2 — Check for duplicates

Call `unity_list_assets` with `kind: "sprite"` to get the current inventory. Compare against
the list of sprites to download. If some already exist, tell the user and ask
whether to skip or re-download them.

### Step 3 — Download

Call `unity_save_sprites` (batch) with:
- `fileKey` — the Figma file key
- `sprites` — array of `{nodeId, spriteName}` for each sprite to download
- `scale` — **MUST be 1**. Do NOT change this value. Only use a different scale if the user explicitly requests it.
- `savePath` — use the user's requested path, or omit to use the default from settings

This fetches all images from Figma in a single API call and imports them into Unity
in one batch (using `AssetDatabase.StartAssetEditing`/`StopAssetEditing`).
Component keys are resolved automatically from cached Figma data for correct
sprite map registration.

### Step 4 — Report

Present a summary table in the user's language:

```
## Sprite Download Report

Downloaded: 12 / 14
Skipped (already existed): 2

| # | Name          | Figma Node   | Unity Path                        | Status |
|---|---------------|--------------|-----------------------------------|--------|
| 1 | icon-search   | 123:456      | Assets/UI/Sprites/icon-search.png | ✓      |
| 2 | icon-settings | 123:789      | Assets/UI/Sprites/icon-settings.png | ✓    |
| 3 | bg-header     | 124:100      | —                                 | ✗ Error: ... |
| ...                                                                              |
```

If any downloads failed, list the errors and suggest retrying those specific nodes.

## Name sanitization

`spriteName` must be in **kebab-case** — this is the asset file name in Unity.
The original Figma node name is preserved automatically in the FigmaSpriteMap
(`figmaName` field), so prefab builds can look up sprites by their Figma name.

When deriving `spriteName` from a Figma node name:

- Replace spaces, slashes, and underscores with hyphens
- Remove characters not allowed in file names
- Convert to lowercase
- Collapse multiple hyphens into one
- Preserve meaningful prefixes like `icon-`, `bg-`, `img-`

Example: `"Icon / Search / 24px"` → `"icon-search-24px"`

## Scale guidance

- **1x** — default, actual Figma pixel size
- **2x** — higher quality for retina/high-DPI screens
- **3x** — for cases where maximum quality is needed
- **4x** — rarely needed, creates very large files

**MUST be 1x.** Do NOT use any other scale unless the user explicitly requests it.
