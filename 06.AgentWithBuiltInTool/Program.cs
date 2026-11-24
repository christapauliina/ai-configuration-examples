// ============================================================================
// Giving the model the ability to search the internet via the built-in web search tool.
// 
// Test:
// 1. greet the model briefly - notice the token count. Tool's json schema goes with every API call, even if you don't use the tool
// 2. Ask the model a question about the latest  information that requires an internet search. Notice the token consumption - tool results are also stored in history and sent to the model with each prompt, which increases token consumption. 
//
// Tip: You can remove tool results from history if you want to limit token consumption, but then the model will not have access to the tool results in future prompts. Remember to delete both the tool call and tool result, or you might get an error from the model about a missing tool call for a tool result in history.
//
// Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
// ============================================================================

using AIConfiguration.Common;
using Microsoft.Extensions.AI;

IChatClient client = ClientFactory.CreateResponsesClient();
Console.WriteLine("06. Agent with Built-in Tool");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

List<ChatMessage> history = [];

// Giving chat options to the model to use a built-in tool for web browsing. 
ChatOptions options = new()
{
    Tools = [new HostedWebSearchTool()]
};

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    history.Add(new ChatMessage(ChatRole.User, prompt!));

    // Pass options so the model knows it has access to HostedWebSearchTool
    ChatResponse response = await client.GetResponseAsync(history, options);

    // Add all response messages (including tool call metadata) to history
    history.AddRange(response.Messages);

    Console.WriteLine($"\nAI: {response.Text}");

    // Print toolcalls, toolcall results and source citations if any were returned by the model in the response. In next demos we will use AIConfiguration.Common.ResponseExtension.PrintToolUsage to print tool usage.
    foreach (var content in response.Messages.SelectMany(m => m.Contents))
    {
        switch (content)
        {
            case ToolCallContent call:
                Console.WriteLine($"\n[Tool Call] {call.GetType().Name} (CallId: {call.CallId})");
                break;

            case TextContent text when text.Annotations is { Count: > 0 }:
                foreach (var citation in text.Annotations.OfType<CitationAnnotation>())
                {
                    Console.WriteLine($"[Source] {citation.Title} -> {citation.Url}");
                }
                break;
        }
    }

    ResponseExtension.PrintTokenUsage(response);
}
