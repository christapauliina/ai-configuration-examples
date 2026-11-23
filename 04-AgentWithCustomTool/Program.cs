// ============================================================================
// Showcasing giving agent a simple custom tool for its use. 
// 
// Test: First greet the model briefly - notice the token count. Tool's json schema goes with every API call, even if you don't use the tool
// Second: Ask the model to count the number of files in your Downloads folder. The model will call the custom tool CountDownloadedFiles() to get the result.
//
// Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
// ============================================================================

using AIConfiguration.Common;
using Microsoft.Extensions.AI;

// Wrap the base client with function invocation middleware.
// Unlike built-in cloud tools, custom tools run locally in our application—this automatically intercepts tool call requests from the model, executes our C# method, and sends the result back.
IChatClient client = ClientFactory.Create()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

Console.WriteLine("04. Model with Custom Tool");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

List<ChatMessage> history = [];


// Giving chat options to the model to use a custom tool for counting files in Downloads folder.
ChatOptions options = new()
{
    Tools = [
        AIFunctionFactory.Create(
            CountDownloadedFiles,
            name: "CountDownloadedFiles",
            description: "Counts the number of files in the Downloads folder")
    ]
};

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    history.Add(new ChatMessage(ChatRole.User, prompt!));

    ChatResponse response = await client.GetResponseAsync(history, options);

    history.AddRange(response.Messages);

    Console.WriteLine($"\nAI: {response.Text}");

    ResponseExtension.PrintToolUsage(response); 

    ResponseExtension.PrintTokenUsage(response);
}

// Creating a method that returns number of files in downloaded folder. This will be used as a custom tool for the model to call.
static int CountDownloadedFiles()
{
    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    if (!Directory.Exists(path)) return 0;

    return Directory.EnumerateFiles(path)
        .Count(f => !Path.GetFileName(f).StartsWith('.') && Path.GetFileName(f) != "desktop.ini");
}
