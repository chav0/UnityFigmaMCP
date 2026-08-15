---
name: setup-project
description: >
  Diagnose and verify the UnityFigmaMCP project setup — Figma token, Unity Editor connection,
  pipeline configuration, asset bindings. Use this skill whenever the user asks about project
  status, whether everything is configured correctly, wants to check connections, or is getting
  started with the project for the first time. Also use it when the user encounters connection
  errors, missing pipelines, or wants to understand what's already set up before starting work.
  Trigger phrases: "setup", "check project", "is everything connected", "status", "diagnose",
  "what's configured", "getting started", "check figma token", "check unity connection".
---

# Setup Project — UnityFigmaMCP Environment Check

This skill runs a diagnostic sequence to verify that all parts of the UnityFigmaMCP bridge
are properly configured and connected. It checks each layer of the stack and produces a
clear status report with actionable next steps.

## Why this matters

UnityFigmaMCP has three independent systems that need to be working together:
the Figma API (token + network), the MCP sidecar server, and the Unity Editor plugin
connected via SignalR. A problem in any layer blocks the entire workflow, and the symptoms
can be confusing — so a structured check saves time.

## Diagnostic sequence

Run these checks in order. Each step depends on the previous one succeeding, but
continue through all steps even if some fail — the user needs the full picture.

### Step 1 — Server, Figma token and Unity connection

Call `status`. One response covers three things:

- `figma.tokenConfigured` / `figma.tokenValid` — whether `FIGMA_ACCESS_TOKEN` is set
  and actually works. `tokenValid` is true only after a real API call succeeded, so
  it catches expired and revoked tokens, not just missing ones. `figma.user` shows
  who it belongs to.
- `unity.editorConnected` — whether a Unity Editor is attached to the SignalR hub,
  plus `unity.hubUrl` for the address the Editor plugin connects to.
- `protocolVersion` and a `hint` naming the next concrete action when something is off.

If `ready` is true, both halves work. Otherwise note what failed and continue —
the remaining checks are still informative.

### Step 2 — Unity Editor round-trip

Call `unity_get_pipelines`.

`status` already reported whether an Editor is registered on the hub, but this is the
first call that actually travels the full path — MCP → server → SignalR → Unity Editor
and back — and returns data the Editor had to produce. A connected-but-wedged Editor
(blocked on a modal dialog, mid-compile) shows up here and not in `status`.

### Step 3 — Pipeline configuration

From the `unity_get_pipelines` response, check:
- Are there any pipelines configured?
- List each pipeline's ID and steps

If no pipelines exist, the user needs to create at least one in Unity via
the FigmaAutoLayoutSettings asset (Assets/UnityFigmaMCP/Editor/FigmaAutoLayoutSettings).

### Step 4 — Existing assets

Call `unity_list_assets` twice in parallel — once with `kind: "prefab"`, once with
`kind: "sprite"` — to survey what's already in the project.

Report:
- Number of prefabs and how many are bound to Figma components
- Number of sprites and how many are bound in the sprite map
- If both are empty, this is likely a fresh project

## Status report format

Present results as a checklist in the user's language.

```
## UnityFigmaMCP — Project Status

### Connections
- [x] Figma token — valid (user: handle, email)
  OR
- [ ] Figma token — not set / rejected by the API
- [x] Unity Editor — connected (hub: http://127.0.0.1:52802/hubs/unity-mcp)
  OR
- [ ] Unity Editor — not connected (open Tools > Unity Figma MCP and press Connect)

### Pipelines
- Pipeline "default" — 7 steps (Text, RectTransform, Image, ...)
  OR
- ⚠️  No pipelines configured

### Assets
- Prefabs: 12 total, 8 bound to Figma components
- Sprites: 45 total, 30 bound in sprite map
  OR
- No assets yet (fresh project)

### Next steps
1. ...
```

## Suggesting next steps

Based on what's missing or empty, suggest concrete actions:

- **No Figma token:** "Add `FIGMA_ACCESS_TOKEN` to the `env` block of the MCP server entry in your client config, using a personal access token from figma.com/developers, then restart the client"
- **Token set but rejected:** "The token is present but the Figma API refused it — it's likely expired or revoked. Generate a new one at figma.com/developers"
- **Unity not connected:** "In Unity, open Tools > Unity Figma MCP and press Connect"
- **Editor connected but `unity_get_pipelines` hangs:** "Unity is attached but not answering — check for a modal dialog or an in-progress compile in the Editor"
- **No pipelines:** "Create a pipeline profile in the FigmaAutoLayoutSettings asset"
- **No assets, everything connected:** "Ready to go! Try `/build-prefab` to build your first prefab from Figma"
- **Everything green:** "All systems operational. You can use `/build-prefab` and `/download-sprites`. Use `/bind-figma-unity` only if the user asks to link an existing (old) UI kit that is not already bound to Figma"
