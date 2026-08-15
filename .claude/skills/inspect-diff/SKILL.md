---
name: inspect-diff
description: >
  Compare a Unity prefab with its Figma source and report all differences without modifying
  anything. Read-only inspection — shows structural and property diffs between what's in
  Figma and what's in Unity. Use this skill whenever the user wants to see what changed,
  check if a prefab is up to date, review differences before updating, or audit a prefab
  against its design. Trigger phrases: "compare prefab", "inspect diff", "what changed",
  "diff figma unity", "check differences", "is prefab up to date".
---

# Inspect Diff — Figma vs Unity Comparison

This skill compares a Unity prefab with its Figma source node and produces a detailed
diff report. Nothing is modified — this is purely read-only inspection. Use it to
understand what's out of sync before deciding whether to update.

## Input

The user needs to provide:
- **Prefab path** — Unity asset path (e.g. `Assets/UI/Prefabs/MyScreen.prefab`)
- **Figma file key** — the key from the Figma URL
- **Node ID** — the Figma node this prefab was built from

## Workflow

### Step 1 — Fetch both sides

Do these in parallel:
- `figma_get_node` + `figma_get_node_names` — current Figma tree
- `unity_get_hierarchy` — current Unity prefab hierarchy

### Step 2 — Compare

Walk both trees in parallel and compare by matching node names/paths.

Check these categories:

**Structure:**
- Nodes in Figma but not in Unity (added in design)
- Nodes in Unity but not in Figma (removed in design, or added manually in Unity)
- Sibling order differences

**Text properties:**
- Content, font size, font family, font style, color, alignment

**RectTransform:**
- Size (width, height), position, anchors, pivot

**Image:**
- Color, sprite reference, image type

**Layout:**
- Layout mode, spacing, padding, child alignment

**ContentSizeFitter:**
- Horizontal/vertical fit modes

**Active state:**
- Enabled/disabled mismatches

### Step 3 — Report

Present in the user's language. Group by severity — structural changes first,
then property changes.

```
## Figma vs Unity — Diff Report

Prefab: Assets/UI/Prefabs/MyScreen.prefab
Source: figma.com/file/<key>, node 123:456

### Summary
- 2 structural changes
- 5 property changes
- Prefab is NOT up to date

### Structural differences

Added in Figma (missing in Unity):
- Card/Badge
- Footer/SocialLinks

Only in Unity (not in Figma):
- Card/CustomOverlay — possibly added manually

### Property differences

| Node           | Property   | Figma        | Unity        |
|----------------|------------|--------------|--------------|
| Header/Title   | text       | "New Title"  | "Old Title"  |
| Header/Title   | fontSize   | 18           | 16           |
| Header/Title   | color      | #111111      | #333333      |
| Card           | width      | 320          | 300          |
| Content        | spacing    | 12           | 8            |
```

If everything matches, say so clearly:

```
## Figma vs Unity — Diff Report

Prefab: Assets/UI/Prefabs/MyScreen.prefab
Source: figma.com/file/<key>, node 123:456

✓ Prefab is up to date — no differences found.
```

At the end, if there are differences, suggest: "Use `/update-prefab` to apply these changes."
