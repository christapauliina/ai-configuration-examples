# AI Configuration Examples (.NET 10)

Eleven small C# console apps that show, one concept at a time, how an LLM becomes an agent: from a stateless call to conversation memory, system prompts, reasoning control, tools, MCP servers and multi-agent orchestration. Built with `Microsoft.Extensions.AI` and the Model Context Protocol (MCP). Every app prints token usage so you can see what each configuration costs.

> ###  Companion Documentation & Visual Showcases
> **No API key or Power BI environment configured?**  
> Explore the full companion site with step-by-step knowledge base, UI screenshots, run traces, and token benchmarks:  
> [AI Configuration Documentation & Showcases](https://christapauliina.github.io/AI-configuration/llm-configuration/)

---

## The projects

Each project isolates one concept. The header comment of every `Program.cs` contains a **Test:** instruction describing what to try and what to notice.

| # | Project | Concept | What to observe | Docs |
|---|---|---|---|---|
| 01 | `01-StatelessModel` | Single-turn inference, `IChatClient` | The model forgets your name between prompts | [Memory](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/01.model-memory) |
| 02 | `02-ModelWithHistory` | Application-side conversation state | Memory works, input tokens grow every turn | [Memory](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/01.model-memory) |
| 03 | `03-ModelWithSystemPrompt` | `ChatOptions.Instructions` | Off-topic questions are declined | [System prompt](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/02.model-systemprompt) |
| 04 | `04-ModelWithReasoningControl` | `ReasoningOptions`, Responses API | Reasoning tokens and trace, low vs high | [Parameters](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/03.model-parameters) |
| 05 | `05-ModelWithParameters` | `Temperature`, `TopP`, `MaxOutputTokens` | Deterministic vs creative output | [Parameters](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/03.model-parameters) |
| 06 | `06-AgentWithBuiltInTool` | `HostedWebSearchTool` | Token cost of hosted tools, citations | [Tools](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/04.agent-tools) |
| 07 | `07-AgentWithCustomTool` | `AIFunctionFactory`, `.UseFunctionInvocation()` | Local tool call, small token footprint | [Tools](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/04.agent-tools) |
| 08 | `08-AgentWithExternalMCP` | MCP client (Power BI) | Large tool list, large token overhead | [MCP](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/05.agent-mcp) |
| 09 | `09-CustomMCP` | Building an MCP server (`[McpServerTool]`) | Two focused tools over stdio | [MCP](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/05.agent-mcp) |
| 10 | `10-AgentWithCustomMCP` | Agent using the server from 09 | Same task, a fraction of the tokens | [MCP](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/05.agent-mcp) |
| 11 | `11-MultiAgentOrchestration` | Researcher → Analyst → Writer | Per-agent token usage, context isolation | [Orchestration](https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/06.advanced-orchestration) |

### Shared library: `AIConfiguration.Common`
- `ClientFactory` — returns an `IChatClient` for either the Chat Completions API (`Create()`) or the Responses API (`CreateResponsesClient()`), configured from `.env`.
- `ResponseExtension` — helpers that print token usage, reasoning traces and tool calls.

---

## Getting started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An Azure OpenAI / Microsoft Foundry or OpenAI-compatible endpoint with an API key
- A model deployment. Demos 04 and 05 assume a **reasoning-capable model** (the docs use GPT-5.2). Demos 01–03 work with simpler chat models.
- *(Demo 08 only)* [Power BI Desktop](https://powerbi.microsoft.com/desktop/) and the [Power BI Modeling MCP server](https://github.com/microsoft/powerbi-modeling-mcp/releases)

### Configure

```bash
git clone https://github.com/christapauliina/ai-configuration-examples.git
cd ai-configuration-examples
cp .env.example .env     # Windows PowerShell: copy .env.example .env
```

Edit `.env`:

```env
ENDPOINT=https://your-resource.services.ai.azure.com/openai/v1/
API_KEY=your-api-key-here
MODEL=your-deployment-name
POWERBI_MCP_PATH=C:\path\to\powerbi-modeling-mcp.exe   # demo 08 only
```

`ClientFactory` loads this file automatically (via `DotNetEnv`). `.env` is git-ignored, never commit your key.

### Run

```bash
dotnet run --project 01-StatelessModel
dotnet run --project 07-AgentWithCustomTool
dotnet run --project 10-AgentWithCustomMCP      # starts 09-CustomMCP itself
dotnet run --project 11-MultiAgentOrchestration
```

Type `quit` to exit the chat-style demos.

**Demo 08** needs Power BI Desktop running with [`PowerBi/Penguins.pbix`](PowerBi/) open (Palmer Penguins sample data, see `PowerBi/README.md`).

> **Note:** The demos are written for clarity, not production. They intentionally leave out error handling, retries, logging and tests.

---

## Known limitations / roadmap

- No automated tests (planned:  unit tests for `AIConfiguration.Common`).
- Demo 10 launches demo 09 via a relative project path, so run it from the repository root.  

## License
MIT
