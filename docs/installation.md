# Installation Guide

UnityFigmaMCP has three parts. Install them in this order.

## Prerequisites

| Dependency | Version | Download |
|-----------|---------|----------|
| Unity | 2021.3+ | [unity.com](https://unity.com/download) |
| .NET SDK | 8.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| MCP client | any | [Claude Code](https://claude.ai/download), Cursor, Windsurf, or any MCP-compatible agent |
| Figma access token | — | [Figma API docs](https://www.figma.com/developers/api#access-tokens) |

## Part 1 — MCP Server

The MCP server is a .NET 8 application that bridges AI agents to Unity and Figma.
Clone it **outside** your Unity project — it's a standalone server, not a Unity asset.

### Clone

```bash
git clone https://github.com/chav0/UnityFigmaMCP.git
```

> **Important:** Don't clone this inside your Unity project's Assets folder. The Unity
> package is installed separately via UPM (see Part 2). The cloned repo is only for
> the MCP server — they live in different places and don't conflict.

### Get a Figma Token

<img width="480" height="772" alt="FigmaToken" src="https://github.com/user-attachments/assets/840ca07d-32c8-498e-871f-63e6a3407ed1" />

1. Go to [Figma Settings > Personal Access Tokens](https://www.figma.com/developers/api#access-tokens)
2. Create a new token with **read** access to your files
3. Copy the token — you'll need it in the next step

### Configure your MCP client

Add the server to your agent's MCP config. The format is the same for most clients.

**Claude Code** (`~/.claude.json` or project `.mcp.json`):

```json
{
  "mcpServers": {
    "unity-figma-mcp": {
      "command": "/absolute/path/to/UnityFigmaMCP/UnityFigmaMCPServer/run-mcp.sh",
      "env": {
        "FIGMA_ACCESS_TOKEN": "figd_your_token_here"
      }
    }
  }
}
```

**Claude Desktop**:

Same JSON, placed in the app config file:

- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`

**Cursor / Windsurf / other MCP clients** — consult your agent's documentation for where
to place MCP server configs. The `command` and `env` fields are the same.

#### macOS / Linux

Use `run-mcp.sh`. Make sure it's executable:

```bash
chmod +x /path/to/UnityFigmaMCP/UnityFigmaMCPServer/run-mcp.sh
```

#### Windows

Use `run-mcp.bat`:

```json
{
  "mcpServers": {
    "unity-figma-mcp": {
      "command": "C:\\path\\to\\UnityFigmaMCP\\UnityFigmaMCPServer\\run-mcp.bat",
      "env": {
        "FIGMA_ACCESS_TOKEN": "figd_your_token_here"
      }
    }
  }
}
```

The server builds itself on first launch — no manual `dotnet build` needed.

## Part 2 — Unity Dependencies

The Unity package requires three dependencies. Install them first via
**Window > Package Manager > + > Add package from git URL...**:

### 2.1 NuGetForUnity

A NuGet package manager for Unity. Paste this URL:

```
https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity
```

### 2.2 SerializeReference Extensions

Required for the pipeline step selector in the Inspector. Paste this URL:

```
https://github.com/mackysoft/Unity-SerializeReferenceExtensions.git?path=Assets/MackySoft/MackySoft.SerializeReferenceExtensions
```

### 2.3 SignalR Client (via NuGet)

The Unity package uses SignalR to communicate with the MCP server. Install it through NuGetForUnity:

1. Go to **NuGet > Manage NuGet Packages**
2. Search for `Microsoft.AspNetCore.SignalR.Client`
3. Click **Install**

## Part 3 — Unity Package

Now install the package itself via **Window > Package Manager > + > Add package from git URL...**:

```
https://github.com/chav0/UnityFigmaMCP.git?path=UnityFigmaMCPPackage
```

Click **Add**.

To pin a specific version, append a git tag:

```
https://github.com/chav0/UnityFigmaMCP.git?path=UnityFigmaMCPPackage#v0.1.0
```

### Manual install (manifest.json)

Open `Packages/manifest.json` in your Unity project and add:

```json
{
  "dependencies": {
    "com.hugglebit.unity.figma.mcp": "https://github.com/chav0/UnityFigmaMCP.git?path=UnityFigmaMCPPackage#v0.1.0"
  }
}
```

### Verify connection

<img width="416" height="483" alt="Connection" src="https://github.com/user-attachments/assets/be689d59-9e4d-4cf4-b3e0-e86d7095977a" />

When Unity opens, the package connects to the MCP server's SignalR hub on `localhost:52802`.
You'll see a console message when the connection is established.

If the MCP server isn't running yet, the package will retry automatically when a tool is invoked.

## Part 4 — Configure a Layout Pipeline

<img width="398" height="273" alt="LayoutPipeline" src="https://github.com/user-attachments/assets/180c7fa5-a18b-459f-a71a-41a45c80d6e5" />

The build pipeline needs at least one layout profile to know which steps to run.

1. Open the **FigmaAutoLayoutSettings** asset in the Inspector
   (created automatically at `Assets/UnityFigmaMCP/Editor/FigmaAutoLayoutSettings.asset`)
2. In the **Pipelines** list, click **+** to add a pipeline
3. Give it an **Id** (e.g. `Screen`) and a **Description**
4. Add **Pipeline Steps** — a basic set for screens:

| # | Step |
|---|------|
| 0 | Text Pipeline Step |
| 1 | Rect Transform Pipeline Step |
| 2 | Image Pipeline Step |
| 3 | Vertical Group Pipeline Step |
| 4 | Horizontal Group Pipeline Step |
| 5 | Grid Pipeline Step |

You can add **Content Size Fitter Pipeline Step** if your designs use hug-content sizing. 
You can also add your own steps by inheriting from FigmaLayoutPipelineObjectStepBase and insert them in pipeline.

Without a configured pipeline, `unity_build_prefab` will fail. You can create multiple
pipelines for different use cases (screens, popups, HUD elements) and select them at build time.

Also set the **Prefab Folder** and **Sprites Folder** to tell the package where to save
generated assets (e.g. `Assets/Prefabs/UI` and `Assets/Sprites/UI`).

## Part 5 — Bind Your UI Kit

Before building or updating prefabs, bind your existing sprites and prefabs to their
Figma components. This is how the build pipeline knows which Unity asset corresponds
to which Figma component — without it, builds will have missing references and the
agent will regenerate assets instead of reusing existing ones.

Use the `/bind-figma-unity` skill, or call `unity_bind` manually. For example:

```
Bind my UI kit sprites and prefabs to Figma components from file abc123, node 100:200
```

You only need to do this once per UI kit. After binding, all subsequent builds and
updates will resolve components and sprites automatically.

## Part 6 — AI Skills (Optional)

Skills are Markdown files that describe multi-step workflows. They teach the agent
how to use the MCP tools in the right order for common tasks.

Skills work with **any agent** that supports instruction/prompt files — not just Claude Code.
They're plain Markdown with no agent-specific dependencies.

### Claude Code

Copy the skills into your project:

```bash
cp -r /path/to/UnityFigmaMCP/.claude/skills/* /path/to/your-project/.claude/skills/
```

### Other agents (Cursor, Windsurf, etc.)

Copy the skill Markdown files into your agent's instruction or prompt directory.
Each skill is a standalone file — read the YAML frontmatter for the skill name
and description, and the body for the workflow steps.

### Available skills

| Command | Description |
|---------|-------------|
| `/build-prefab` | Build a prefab from a Figma frame or component |
| `/update-prefab` | Update an existing prefab to match Figma changes |
| `/download-sprites` | Export and import sprites from Figma |
| `/bind-figma-unity` | Link existing assets to Figma components |
| `/inspect-diff` | Compare prefab vs. Figma source |
| `/setup-project` | Check that everything is connected |

## Troubleshooting

### "Figma token is invalid"

- Make sure `FIGMA_ACCESS_TOKEN` is set in your MCP server config
- Check that the token has read access to the file you're trying to fetch
- Tokens starting with `figd_` are personal access tokens (correct)

### "Unity Editor is not connected"

- Make sure Unity is open with a project that has the package installed
- Check the Unity Console for connection errors
- The SignalR hub runs on `localhost:52802` — make sure the port is not blocked

### "dotnet: command not found"

- Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Restart your MCP client (Claude Code, Cursor, etc.) after installing — the client
  spawns the server process, so it needs to pick up the new PATH

### Package not showing in UPM

- Make sure you're using Unity 2021.3 or later
- Check that the git URL is correct — it must include `?path=UnityFigmaMCPPackage`
- If using a tag, make sure it exists: `#v0.1.0`
