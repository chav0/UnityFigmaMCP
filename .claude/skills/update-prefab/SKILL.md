---
name: update-prefab
description: >
  Update an existing Unity prefab to match changes in its Figma source. Compares the current
  prefab hierarchy and properties with the Figma node, then applies structural and property
  changes — text, colors, sizes, layout, hierarchy. Does not touch sprites. Use this skill
  whenever the user wants to sync a prefab with Figma after design changes, refresh a prefab,
  apply Figma updates, or bring a prefab up to date. Trigger phrases: "update prefab",
  "sync prefab", "refresh prefab", "apply figma changes".
---

# Update Prefab — Sync with Figma Source

This skill compares an existing Unity prefab with its Figma source node and applies
the differences. It handles structural changes (added/removed children) and property
changes (text content, colors, sizes, fonts, layout settings). Sprites are not modified —
only properties and structure.

## When to use

The designer made changes in Figma and the developer needs to bring the Unity prefab
in sync. Rather than rebuilding from scratch (which would lose any manual Unity-side
tweaks), this skill applies only the delta.

## Input

The user needs to provide:
- **Prefab path** — Unity asset path (e.g. `Assets/UI/Prefabs/MyScreen.prefab`)
- **Figma file key** — the key from the Figma URL
- **Node ID** — the Figma node this prefab was built from

If the user doesn't know the node ID, they may need to look it up in Figma.
If the prefab was built with `/build-prefab`, the node ID might be discoverable
from the component map via `unity_list_assets` with `kind: "prefab"`.

## Workflow

### Step 1 — Fetch both sides

Do these in parallel:
- `figma_get_node` + `figma_get_node_names` — get the current Figma tree
- `unity_get_hierarchy` — get the current Unity prefab hierarchy

### Step 2 — Compare

Walk both trees and identify differences:

**Structural changes:**
- Nodes present in Figma but missing in Unity → need to be created
- Nodes present in Unity but missing in Figma → candidates for removal
  (ask the user before deleting — they might be intentional Unity-side additions)

**Property changes per node** (compare by matching node names/paths):
- **Text:** content, font size, font family, font style, color, alignment
- **RectTransform:** size, position, anchors, pivot
- **Image:** color, sprite path, image type
- **Layout:** layout mode, spacing, padding, alignment
- **ContentSizeFitter:** horizontal/vertical fit modes
- **Active state:** enabled/disabled

### Step 3 — Present the diff

Before applying anything, show the user what will change. Group by change type:

```
## Changes detected

### Text changes
- Header/Title: "Old Title" → "New Title"
- Header/Subtitle: fontSize 14 → 16, color #333 → #222

### Size changes
- Card: 300×200 → 320×220
- Card/Icon: 24×24 → 32×32

### Layout changes
- Content: spacing 8 → 12, paddingTop 16 → 20

### New nodes (will be created)
- Card/Badge
- Footer/SocialLinks

### Removed in Figma (confirm before deleting)
- Card/OldLabel
```

Ask the user to confirm before proceeding. They might want to skip some changes
or keep Unity-side additions that aren't in Figma.

### Step 4 — Apply changes

Send the whole update as a single `unity_edit_prefab` call. The edits run in the
order you list them and the prefab is saved only if every one succeeds, so a bad
path fails the batch cleanly instead of leaving the asset half-updated.

Order the edits like this, because later steps reference paths the earlier ones create:

1. **`create`** — new nodes first, so the paths exist for the edits below
2. **`reparent`** — moves and reordering
3. **Component ops** — `text`, `rectTransform`, `image`, `verticalLayout`,
   `horizontalLayout`, `gridLayout`, `contentSizeFitter`, plus `setActive`
4. **`delete`** — last, and only for removals the user confirmed

Deleting last matters: an earlier delete would invalidate the paths of anything
nested under it that you still needed to touch.

Example shape:

```json
[
  { "op": "create", "path": "Content", "name": "Badge" },
  { "op": "rectTransform", "path": "Content/Badge", "rectTransform": { "width": 24, "height": 24 } },
  { "op": "text", "path": "Header/Title", "text": { "text": "Updated", "fontSize": 18 } },
  { "op": "delete", "path": "Card/OldLabel" }
]
```

To strip a component rather than update it, set `"remove": true` and omit the payload.

### Step 5 — Report

Present the report in the user's language:

```
## Update Report

Prefab: Assets/UI/Prefabs/MyScreen.prefab
Source: figma.com/file/<key>, node 123:456

Applied:
- 3 text changes
- 2 size changes
- 1 layout change
- 2 new nodes created

Skipped:
- Card/OldLabel — kept (user choice)

No sprite changes were made. If sprites need updating, use `/download-sprites`.
```

## Important notes

- This skill does NOT update sprites. If a Figma node's image fill changed, the user
  should re-download the sprite with `/download-sprites` separately.

- Always show the diff and get confirmation before applying destructive changes
  (deletions, major structural rearrangements). Property updates (text, colors, sizes)
  can be applied without confirmation since they're easily reversible.

- When comparing node names between Figma and Unity, use flexible matching —
  the names might have been sanitized during the initial build.
