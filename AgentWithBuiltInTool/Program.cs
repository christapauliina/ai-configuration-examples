// ============================================================================
// Giving the model a ability to search internet vie built in tool Bing Grounding.
// 
// Test: Ask the model a question about latest news or information that requires internet search. 
// Notice the token consumption - tool results are also stored in history and sent to the model with each prompt, whixh increases token consumption. 
//
// Tip: You can remove tool results from history if you want to limit token consumption, but then the model will not have access to the tool results in future prompts. Remember to delete both the toolcall and toolresult, or you might get an error from the model about missing toolcall for a toolresult in history.
//
// Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
// ============================================================================

using AIConfiguration.Common;
using Microsoft.Extensions.AI;

IChatClient client = ClientFactory.Create();
Console.WriteLine("03. Model with Built-in Tool");
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
                Console.WriteLine($"[Tool Call] {call.GetType().Name} (CallId: {call.CallId})");
                break;

            case ToolResultContent result:
                Console.WriteLine($"[Tool Result] {result.GetType().Name} (CallId: {result.CallId})");
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
