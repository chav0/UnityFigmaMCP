# AI Skills Guide

Skills are pre-built workflows that teach an AI agent to orchestrate MCP tools
in the right order for a specific task. Each skill is a plain Markdown file —
no agent-specific dependencies, works with any MCP-compatible agent.

## Using skills

**Claude Code** — type the skill name as a slash command:

```
/build-prefab
```

**Other agents** (Cursor, Windsurf, etc.) — copy the skill Markdown files into
your agent's instruction or prompt directory. The agent reads the workflow steps
and follows them when you describe the task.

The agent loads the skill instructions and follows the workflow — you guide it
with a Figma URL, prefab name, and any preferences.

## Available Skills

### `/build-prefab`

Build a Unity UI prefab from a Figma node.

**Workflow:**
1. Fetch the Figma node tree
2. Show a preview image and hierarchy
3. Rename generic Figma names (Frame 32, Group 5) to meaningful names in Figma
4. Choose a layout pipeline
5. Build the prefab in Unity
6. Inspect the result hierarchy
7. Handle variants (for component sets)
8. Report — hierarchy, variants, missing sprites

**Input:** Figma URL or file key + node ID, prefab name.

**Supports:** Frames, Components, Component Sets (with automatic variant prefabs).

### `/update-prefab`

Update an existing prefab to match changes in its Figma source.

Compares the current prefab with the Figma node, then applies structural and property
changes — text, colors, sizes, layout, visibility. Does not touch sprites.

**Input:** Prefab asset path, Figma URL.

### `/download-sprites`

Export Figma nodes as PNG images and import them into Unity as sprite assets.

Fetches the design, identifies image nodes, exports them via the Figma API, and
imports them into Unity with proper sprite settings and Figma key registration.

**Input:** Figma URL or file key + node IDs.

### `/bind-figma-unity`

Link existing Unity prefabs and sprites to Figma components.

This is an **opt-in** skill for legacy UI kits. If your assets were built with
this tool, they're already bound automatically. Use this only when you have
existing assets that need to be connected to Figma components for the first time.

**Input:** Figma URL, then the skill scans and matches by name.

### `/inspect-diff`

Compare a Unity prefab against its Figma source and report all differences.

Read-only — doesn't modify anything. Shows structural diffs (missing/extra nodes)
and property diffs (text, colors, sizes, visibility).

**Input:** Prefab asset path, Figma URL.

### `/setup-project`

Diagnose and verify the project setup.

Checks:
- Figma access token validity
- Unity Editor connection
- Pipeline configuration
- Asset folder settings

Run this when getting started or when something isn't working.

## Installing skills

**Claude Code:**

```bash
cp -r /path/to/UnityFigmaMCP/.claude/skills/* /path/to/your-project/.claude/skills/
```

**Other agents:** copy the `.md` files from `.claude/skills/*/SKILL.md` into
your agent's prompt/instruction directory. The YAML frontmatter contains the skill
name and trigger description; the body contains the workflow.

## Customizing skills

Skills are plain Markdown files. You can:

- Edit the workflow steps
- Add project-specific instructions
- Create new skills for your own workflows

Each skill file has a YAML frontmatter with `name` and `description` — the description
controls when the agent triggers the skill automatically.
