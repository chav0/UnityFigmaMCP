# Architecture

UnityFigmaMCP has three components that work together.

```
┌─────────────────────────────────────────────────────────────────┐
│                         AI Agent                                │
│                    (Claude Code / Desktop)                       │
│                                                                 │
│  Skills:  /build-prefab  /update-prefab  /download-sprites ...  │
└──────────────────────────┬──────────────────────────────────────┘
                           │ MCP (stdio)
                           │ JSON-RPC tool calls
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│                       MCP Server (.NET 8)                        │
│                                                                  │
│  ┌────────────────┐  ┌─────────────────┐  ┌──────────────────┐  │
│  │  Figma Tools   │  │  Unity Tools    │  │  Status Tools    │  │
│  │  4 tools       │  │  8 tools        │  │  1 tool          │  │
│  └───────┬────────┘  └────────┬────────┘  └──────────────────┘  │
│          │                    │                                   │
│          ▼                    ▼                                   │
│  ┌───────────────┐  ┌────────────────────┐                      │
│  │ Figma REST API│  │ SignalR Hub        │                      │
│  │ (HTTPS)       │  │ localhost:52802    │                      │
│  └───────────────┘  └────────┬───────────┘                      │
└──────────────────────────────┼──────────────────────────────────┘
                               │ SignalR WebSocket
                               ▼
┌──────────────────────────────────────────────────────────────────┐
│                    Unity Editor Package                           │
│                                                                   │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────┐  │
│  │ Command Router   │  │ Command Handlers │  │ Pipeline Steps│  │
│  │ (SignalR client) │  │ Build, Edit,     │  │ RectTransform │  │
│  │                  │  │ Save, Bind, ...  │  │ Text, Image   │  │
│  └──────────────────┘  └──────────────────┘  │ Layout, Grid  │  │
│                                               │ CSF           │  │
│  ┌──────────────────┐  ┌──────────────────┐  └───────────────┘  │
│  │ Component Map    │  │ Sprite Map       │                      │
│  │ (prefab reuse)   │  │ (sprite reuse)   │                      │
│  └──────────────────┘  └──────────────────┘                      │
└──────────────────────────────────────────────────────────────────┘
```

## MCP Server

The server is a .NET 8 console application that speaks two protocols simultaneously:

- **MCP (stdin/stdout)** — the AI agent calls tools via JSON-RPC over stdio
- **SignalR (HTTP/WebSocket)** — the Unity Editor connects as a SignalR client

When an AI agent calls a Unity tool (e.g., `unity_build_prefab`), the server serializes
the command and sends it to Unity via SignalR. Unity executes the command and returns
the result. The server forwards the result back to the agent.

Figma tools talk directly to the Figma REST API — they don't go through Unity.

### Multiple server instances

If the SignalR port (52802) is already in use by another MCP server instance, the new
instance detects this and routes Unity commands through the existing hub. This way
multiple AI agents can share a single Unity Editor connection.

## Unity Editor Package

The package runs inside the Unity Editor process. It:

1. **Connects** to the MCP server's SignalR hub on startup
2. **Routes** incoming commands to the right handler
3. **Executes** commands on the main thread (required by Unity APIs)
4. **Returns** results as JSON

### Command Handlers

Each MCP tool maps to a command handler in Unity:

| Command | Handler | What it does |
|---------|---------|-------------|
| `BuildPrefabCommand` | `BuildPrefabCommandHandler` | Runs the layout pipeline, saves a prefab |
| `EditPrefabCommand` | `EditPrefabCommandHandler` | Opens prefab, applies edits, saves |
| `SavePrefabCommand` | `SavePrefabCommandHandler` | Extracts and saves a sub-prefab |
| `GetHierarchyCommand` | `GetHierarchyCommandHandler` | Serializes the hierarchy tree |
| `SaveSpritesCommand` | `SaveSpritesCommandHandler` | Imports PNGs, configures sprite settings |
| `BindAssetCommand` | `BindAssetCommandHandler` | Registers assets in component/sprite maps |
| `ListAssetsCommand` | `ListAssetsCommandHandler` | Scans configured asset folders |
| `GetPipelinesCommand` | `GetPipelinesCommandHandler` | Lists pipeline profiles |

### Layout Pipeline

The build pipeline applies a sequence of steps to each GameObject:

| Step | What it does |
|------|-------------|
| `RectTransformPipelineStep` | Position, size, anchors, pivot from Figma constraints |
| `TextPipelineStep` | TextMeshPro text, font size, color, alignment |
| `ImagePipelineStep` | Image component with sprite from sprite map |
| `HorizontalGroupPipelineStep` | HorizontalLayoutGroup from Figma auto-layout |
| `VerticalGroupPipelineStep` | VerticalLayoutGroup from Figma auto-layout |
| `GridPipelineStep` | GridLayoutGroup from Figma grid layout |
| `ContentSizeFitterPipelineStep` | ContentSizeFitter for auto-sizing |

Pipeline profiles are configured in Unity via `FigmaAutoLayoutSettings`.

### Component & Sprite Maps

- **FigmaComponentMap** — maps Figma component keys to Unity prefabs. When the builder
  encounters an INSTANCE node with a known key, it inserts the existing prefab instead
  of rebuilding the subtree.
- **FigmaSpriteMap** — maps Figma keys and names to Unity sprites. Resolves by component
  key first, then falls back to the original Figma node name.

Both maps are ScriptableObject assets stored in your project.

## AI Skills

Skills are Markdown files in `.claude/skills/` that describe multi-step workflows.
When triggered, Claude Code loads the skill and follows its instructions, calling
MCP tools in the right order.

Skills don't add new capabilities — they compose existing tools into reliable
workflows that the agent can execute without improvising each step.

## Data Flow Example

Building a prefab from a Figma frame:

```
Agent: "Build MainScreen from figma.com/file/abc123/..., node 100:200"

1. Agent calls figma_get_node(fileKey="abc123", nodeId="100:200")
   → Server fetches from Figma API, saves JSON locally
   → Returns: "/tmp/figma/abc123/100:200.json"

2. Agent calls unity_build_prefab(fileKey="abc123", nodeId="100:200", prefabName="MainScreen")
   → Server sends BuildPrefabCommand via SignalR
   → Unity reads the saved JSON
   → Unity creates GameObjects in a preview scene
   → Pipeline steps apply layout, text, images
   → Known components get substituted from the component map
   → Prefab saved to Assets/UI/Prefabs/MainScreen.prefab
   → Result returned through SignalR → MCP → Agent

3. Agent calls unity_get_hierarchy(prefabPath="Assets/UI/Prefabs/MainScreen.prefab")
   → Returns the full hierarchy with components for the report
```
