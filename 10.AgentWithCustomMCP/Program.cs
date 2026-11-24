// ========================================================================================================================================================
// Connecting an agent to an custom MCP server 
//
// Test: Greet the agent and notice the token consumption. Then ask the agent to count the number of files in your Downloads folder and the total size of files in your Downloads folder.
//
// Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
// ========================================================================================================================================================

using AIConfiguration.Common;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

IChatClient client = ClientFactory.CreateResponsesClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "CustomDownloadsMcp",
    Command = "dotnet",
    Arguments = ["run", "--project", "../../../../09-CustomMCP/09-CustomMCP.csproj"]
});

await using McpClient mcpClient = await McpClient.CreateAsync(transport);

IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

ChatOptions options = new()
{
    Tools = [.. tools]
};

Console.WriteLine("10. Agent with Custom MCP Server");
Console.WriteLine($"Connected. Loaded {tools.Count} MCP tools: {string.Join(", ", tools.Select(t => t.Name))}\n"); 
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");


List<ChatMessage> history = [];

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    history.Add(new ChatMessage(ChatRole.User, prompt!));

    ChatResponse response = await client.GetResponseAsync(history, options);

    history.Add(new ChatMessage(ChatRole.Assistant, response.Text));

    Console.WriteLine($"\nAI: {response.Text}");
    ResponseExtension.PrintToolUsage(response);
    ResponseExtension.PrintTokenUsage(response);
}