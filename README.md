# AI Configuration & Orchestration Patterns (.NET 10)

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Microsoft.Extensions.AI](https://img.shields.io/badge/Library-Microsoft.Extensions.AI-blue)](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/)
[![Model Context Protocol](https://img.shields.io/badge/Protocol-MCP-green)](https://modelcontextprotocol.io/)
[![Documentation](https://img.shields.io/badge/Docs-Interactive_Guide_%26_Traces-orange)](https://christapauliina.github.io/AI-configuration/)

A progressive series of C# console applications demonstrating modern generative AI architecture in .NET 10 using `Microsoft.Extensions.AI` and the Model Context Protocol (MCP). 

The repository walks step-by-step from raw stateless inference up to enterprise MCP tool servers and multi-agent collaborative workflows.

---

> ###  Companion Documentation & Visual Showcases
> **No API key or Power BI environment configured?**  
> Explore the full companion site with step-by-step knowledge base, UI screenshots, run traces, and token benchmarks:  
>  **[AI Configuration Documentation & Showcases](https://christapauliina.github.io/AI-configuration/)**

---

##  Architectural Progression

Each project isolates a single architectural concept, progressively layering complexity:

| # | Project | Core Concept | Key APIs & Patterns |
|---|---|---|---|
| **01** | `01-StatelessModel` | Single-turn inference | `IChatClient`, raw string prompt, token counting |
| **02** | `02-ModelWithHistory` | Conversation state | `ChatMessage`, `List<ChatMessage>`, context window management |
| **03** | `03-ModelWithSystemPrompt` | Guardrails & persona | `ChatOptions.Instructions`, system prompt boundaries |
| **04** | `04.ModelWithReasoningControl` | Reasoning model control | OpenAI Responses API, `ReasoningOptions`, `ReasoningEffort`, reasoning tokens |
| **05** | `05.ModelWithParameters` | Output variance & determinism | `Temperature`, `TopP`, `MaxOutputTokens`, reasoning bypass |
| **06** | `06.AgentWithBuiltInTool` | Cloud-hosted server tools | `HostedWebSearchTool`, tool schemas, annotations & citations |
| **07** | `07-AgentWithCustomTool` | Local function invocation | `AIFunctionFactory.Create()`, `.UseFunctionInvocation()` middleware |
| **08** | `08-AgentWithExternalMCP` | External MCP client (Power BI) | `StdioClientTransport`, `McpClient.ListToolsAsync()`, DAX query execution |
| **09** | `09-CustomMCP` | Building a C# MCP Server | `ModelContextProtocol.Server`, `[McpServerTool]`, stdio transport |
| **10** | `10.AgentWithCustomMCP` | Consuming custom MCP Server | Orchestrating an agent with the local stdio MCP server from project 09 |
| **11** | `11-MultiAgentOrchestration` | Autonomous agent collaboration | Hierarchical agent pattern: Analyst invokes Researcher tool, feeds Writer agent |

---

## Solution Architecture

All projects build on top of a shared core library:

* **`AIConfiguration.Common`**
  * `ClientFactory`: Factory providing abstracted `IChatClient` instances for both the standard **Chat Completions API** (`/v1/chat/completions`) and the newer **Responses API** (`/v1/responses`).
  * `ResponseExtension`: Centralized diagnostic helpers for token consumption, reasoning traces, and tool call inspection.
* **`PowerBi`**
  * Contains sample semantic model data (`Penguins.pbix`) used for the Power BI MCP demonstration.

---

##  Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* An Azure OpenAI or OpenAI API endpoint with key
* *(Optional)* [Power BI Desktop](https://powerbi.microsoft.com/desktop/) and the [Power BI Modeling MCP server](https://github.com/microsoft/powerbi-modeling-mcp/releases) (for project `08`)

### Configuration

1. Clone the repository:
   ```bash
   git clone https://github.com/christapauliina/ai-configuration-examples.git
   cd ai-configuration-examples
   ```

2. Copy `.env.example` to `.env` in the repository root:
   ```bash
   cp .env.example .env
   ```

3. Populate your `.env` variables:
   ```env
   ENDPOINT=https://your-resource.services.ai.azure.com/openai/v1/
   API_KEY=your-api-key-here
   MODEL=gpt-4o-mini
   POWERBI_MCP_PATH=C:\path\to\powerbi-modeling-mcp.exe
   ```
   *(Variables are automatically resolved by `ClientFactory` across all projects via `DotNetEnv`)*.

---

##  Running the Demos

Run any project directly using the .NET CLI:

```bash
# 1. Basic stateless inference
dotnet run --project 01-StatelessModel

# 2. Local custom tool calling
dotnet run --project 07-AgentWithCustomTool

# 3. Power BI MCP Agent (requires Power BI Desktop open with an .pbix -file)
dotnet run --project 08-AgentWithExternalMCP

# 4. Multi-agent orchestration
dotnet run --project 11-MultiAgentOrchestration
```

> **Note:** The demos are intentionally written for clarity and architectural illustration without production error handling or enterprise logging.