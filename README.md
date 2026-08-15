# UnityFigmaMCP

[![MCP](https://img.shields.io/badge/MCP-Compatible-blue)](https://modelcontextprotocol.io)
[![Unity](https://img.shields.io/badge/Unity-2021.3%2B-black?logo=unity)](https://unity.com)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Release](https://img.shields.io/github/v/release/chav0/UnityFigmaMCP)](https://github.com/chav0/UnityFigmaMCP/releases)

Build Unity UI prefabs from Figma designs using AI agents via [Model Context Protocol](https://modelcontextprotocol.io).

An AI agent reads your Figma file, talks to the Unity Editor in real time, and assembles
prefabs — with layout, text, images, components, and variants — without you writing a
single line of code.

```
┌─────────────┐      MCP (stdio)      ┌─────────────────┐    SignalR     ┌──────────────┐
│  AI Agent   │ ◄───────────────────► │   MCP Server    │ ◄────────────► │ Unity Editor │
│             │                       │    (.NET 8)     │   localhost    │   Package    │
└─────────────┘                       └────────┬────────┘                └──────────────┘
                                               │
                                        Figma REST API
                                               │
                                      ┌────────▼────────┐
                                      │   Figma File    │
                                      └─────────────────┘
```

## How it works

1. You give the agent a Figma URL and say "build a prefab"
2. The MCP server fetches the design tree from Figma
3. It sends the layout data to Unity Editor via SignalR
4. Unity runs the pipeline — RectTransform, Text, Image, Layout Groups, ContentSizeFitter — and saves a `.prefab`
5. Component instances are resolved automatically: if a sprite or prefab is already bound, it gets reused

The whole thing runs locally. Your Figma token stays on your machine.

## Features

**13 MCP tools** for full Figma-to-Unity control:

| Group | Tools | What they do |
|-------|-------|--------------|
| Figma | `figma_get_node`, `figma_get_node_names`, `figma_get_component_info`, `figma_export_image` | Fetch design data, hierarchy, component keys, export PNGs |
| Build | `unity_build_prefab`, `unity_get_pipelines` | Build prefabs from Figma nodes with configurable pipelines |
| Edit | `unity_edit_prefab`, `unity_save_prefab`, `unity_get_hierarchy` | Modify prefabs, extract sub-prefabs, inspect hierarchy |
| Assets | `unity_save_sprites`, `unity_bind`, `unity_list_assets` | Import sprites, bind assets to Figma keys, list project assets |
| Status | `status` | Check Figma token, Unity connection, server health |

**6 AI skills** — ready-made workflows for any MCP-compatible agent:

| Skill | What it does |
|-------|--------------|
| `/build-prefab` | Full Figma-to-Unity prefab pipeline with preview, renaming, and variants |
| `/update-prefab` | Sync an existing prefab with Figma changes |
| `/download-sprites` | Export and import sprites from Figma |
| `/bind-figma-unity` | Link existing Unity assets to Figma components |
| `/inspect-diff` | Compare a prefab against its Figma source (read-only) |
| `/setup-project` | Diagnose connections, tokens, and pipeline config |

**Layout pipeline** with configurable steps:
RectTransform · Text (TextMeshPro) · Image · Horizontal/Vertical Layout Groups · Grid · ContentSizeFitter

## Quick Start

### Prerequisites

- Unity 2021.3 or later
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An MCP-compatible AI agent ([Claude Code](https://claude.ai/download), Cursor, Windsurf, or any other)
- [Figma access token](https://www.figma.com/developers/api#access-tokens)

### 1. Install the MCP Server

Clone this repository **outside** your Unity project (it's not a Unity asset — it's a standalone server):

```bash
git clone https://github.com/chav0/UnityFigmaMCP.git
```

Add the server to your MCP client config.

**Claude Code** (`~/.claude.json` or project `.mcp.json`):

```json
{
  "mcpServers": {
    "unity-figma-mcp": {
      "command": "/path/to/UnityFigmaMCP/UnityFigmaMCPServer/run-mcp.sh",
      "env": {
        "FIGMA_ACCESS_TOKEN": "your-figma-token"
      }
    }
  }
}
```

On Windows, use `run-mcp.bat` instead of `run-mcp.sh`.

### 2. Install Dependencies

Open your Unity project, then install these packages via **Window > Package Manager > + > Add package from git URL**:

1. **NuGetForUnity** — NuGet package manager for Unity:
   ```
   https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity
   ```

2. **SerializeReference Extensions** — required for pipeline step selection in the Inspector:
   ```
   https://github.com/mackysoft/Unity-SerializeReferenceExtensions.git?path=Assets/MackySoft/MackySoft.SerializeReferenceExtensions
   ```

3. **SignalR Client** — open **NuGet > Manage NuGet Packages**, search for `Microsoft.AspNetCore.SignalR.Client` and install it

### 3. Install the Unity Package

In Package Manager, add one more git URL:

```
https://github.com/chav0/UnityFigmaMCP.git?path=UnityFigmaMCPPackage
```

To pin a specific version:

```
https://github.com/chav0/UnityFigmaMCP.git?path=UnityFigmaMCPPackage#v0.1.0
```

The package auto-connects to the MCP server on port `52802` when the Editor starts.

> **Note:** The cloned repo and the Unity package are separate things. The repo contains the MCP server source code; the Unity package is installed independently via UPM from the same git URL but a different path. They don't conflict.

### 4. Configure a Layout Pipeline

Open the **FigmaAutoLayoutSettings** asset (`Assets/UnityFigmaMCP/Editor/`) in the Inspector
and add at least one pipeline. A basic pipeline for screens includes these steps:

> Text · RectTransform · Image · Vertical Layout Group · Horizontal Layout Group · Grid

Also set **Prefab Folder** and **Sprites Folder** to where you want generated assets saved
(e.g. `Assets/Prefabs` and `Assets/Sprites`).

Without a pipeline, `unity_build_prefab` has nothing to run and the build will fail.

### 5. Bind Your UI Kit

Before building prefabs, bind your existing sprites and prefabs to Figma components.
This lets the build pipeline reuse your assets instead of generating placeholders.

Use the `/bind-figma-unity` skill, or call `unity_bind` manually:

```
Bind my UI kit sprites and prefabs to Figma components from file abc123, node 100:200
```

This step is important — without binding, the agent won't know which Unity assets
correspond to which Figma components, and builds will have missing references.

### 6. Add Skills (Optional)

Skills are Markdown workflow files that teach the agent to use the MCP tools effectively.
They work with any agent that supports skill/instruction files — not just Claude Code.

**Claude Code** — copy into your project:

```bash
cp -r /path/to/UnityFigmaMCP/.claude/skills/* /path/to/your-project/.claude/skills/
```

**Other agents** — copy the skill Markdown files into your agent's instruction/prompt directory.
The skills are plain Markdown with no agent-specific dependencies.

### 7. Verify

Open Unity, then ask your agent:

```
Check the project status
```

This runs the `status` tool and confirms both the Figma token and Unity connection are working.

## Documentation

- **[Installation Guide](docs/installation.md)** — detailed setup for macOS and Windows
- **[Tools Reference](docs/tools.md)** — all 13 MCP tools with parameters and examples
- **[Skills Guide](docs/skills.md)** — how to use the 6 AI skills
- **[Architecture](docs/architecture.md)** — how the three components connect

## Updating

**Unity Package** — change the tag in your `Packages/manifest.json`:

```json
"com.hugglebit.unity.figma.mcp": "https://github.com/chav0/UnityFigmaMCP.git?path=UnityFigmaMCPPackage#v0.2.0"
```

**MCP Server** — pull and rebuild:

```bash
cd UnityFigmaMCP
git pull
```

The `run-mcp.sh` script rebuilds automatically on each launch.

**Skills** — re-copy the skill files from the updated repo.

## License

[MIT](LICENSE)
