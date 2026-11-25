/*
===========================================================================================================================
Connecting an agent to an external MCP server (demonstrated with Power BI MCP) to inspect a semantic model and execute DAX queries via standardized tools.
Docs: https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/05.agent-mcp
 
Configure: 
1. Download the Power BI Modeling MCP server (https://github.com/microsoft/powerbi-modeling-mcp/releases).
2. Add the executable path to your .env file as POWERBI_MCP_PATH.
3. Open the target .pbix report file in Power BI Desktop before running this app.

Test: Ask the agent for example: "What tables and measures exist in the raport that is open locally?" or "Compare the average body mass across penguin species."

Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
===========================================================================================================================
*/

using AIConfiguration.Common;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

// Configure the client to automatically execute tool calls locally.
// When the model requests an MCP tool, this intercepts the request, invokes the tool, and feeds the result back.
IChatClient client = ClientFactory.CreateResponsesClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

// Get the path to the Power BI MCP server from the environment variable.
string mcpServerPath = Environment.GetEnvironmentVariable("POWERBI_MCP_PATH")!;

// Configure how our application launches and communicates with the external MCP server.
// StdioClientTransport starts the server as a local background process and talks to it via standard input/output.
var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "PowerBiMcp",
    Command = mcpServerPath,
    Arguments = ["--start"]
});

// Start the MCP server process and establish the protocol handshake.
await using McpClient mcpClient = await McpClient.CreateAsync(transport);

// Query the server for its available tools.
// ListToolsAsync() fetches the tool schemas (names, descriptions, and parameters)
// and wraps them as AITool instances that Microsoft.Extensions.AI can pass directly to
IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

// Giving the tools in MCP to the model via ChatOptions so it knows it has access to the tools in the external MCP server.
ChatOptions options = new()
{
    Tools = [.. tools]
};

List<ChatMessage> history = [];

Console.WriteLine("08. Agent with External MCP Server (Power BI)");
Console.WriteLine($"Connected. Loaded {tools.Count} MCP tools: {string.Join(", ", tools.Select(t => t.Name))}\n"); // printing the availbe tools fo user
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    history.AddRange(new ChatMessage(ChatRole.User, prompt!));

    ChatResponse response = await client.GetResponseAsync(history,options); 

    history.AddRange(new ChatMessage(ChatRole.Assistant, response.Text));  

    Console.WriteLine($"\nAI: {response.Text}");
    ResponseExtension.PrintToolUsage(response);
    ResponseExtension.PrintTokenUsage(response);
}