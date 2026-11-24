/*
===========================================================================================================================
Connecting to a model and creating a system prompt to guide its behavior.
Docs: https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/02.model-systemprompt

Test: First ask the model about food instructions, then ask it to give you instructions for painting a wall. Notice the difference in the model's behavior and responses.

Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
===========================================================================================================================
*/
using AIConfiguration.Common;
using Microsoft.Extensions.AI;

IChatClient client = ClientFactory.Create();

// Creating a system prompt via ChatOptions.Instructions. You can also define it by adding a ChatMessage with ChatRole.System to the history, though that gives you less flexibility when trimming or managing the context window.
var options = new ChatOptions
{
    Instructions = "You are a cooking assistant. You are polite and you answer only questions about food and cooking. If the question is not about food or cooking, you should politely decline to answer."
};

List<ChatMessage> history = [];

Console.WriteLine("03. Model with System Prompt Demo");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    history.Add(new ChatMessage(ChatRole.User, prompt!));

    // Passing the system prompt via options
    ChatResponse response = await client.GetResponseAsync(history, options);

    history.Add(new ChatMessage(ChatRole.Assistant, response.Text));

    Console.WriteLine($"\nAI: {response.Text}");

    ResponseExtension.PrintTokenUsage(response);
}