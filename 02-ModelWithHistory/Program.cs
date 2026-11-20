// ============================================================================
// Connecting to a model and creating a history of interactions, so the model can remember previous prompts and responses.
// 
// Test: Tell the model your name and ask it to remember it. Then ask it for your name again twice - notice the difference in token consumption
// Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
// ============================================================================

using AIConfiguration.Common;
using Microsoft.Extensions.AI;

IChatClient client = ClientFactory.Create();
Console.WriteLine("02. Model with History Demo");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

// Create a list to hold the history of interactions with the model
// Model is still stateless, the application is responsible for keeping track of the history and sending it to the model with each prompt.
List<ChatMessage> history = []; 

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;
   
    // Notice the use of the ChatMessage class instead of string - ChatMessage allows us to specify the role of the message (user, assistant, system)
    // This is important for the model to understand the context of the conversation and respond appropriately.

    history.Add(new ChatMessage(ChatRole.User, prompt!)); 

    ChatResponse response = await client.GetResponseAsync(history);

    history.Add(new ChatMessage(ChatRole.Assistant, response.Text)); // Since the model has no tools or functions at this point, it returns a single message. Otherwise, use history.AddRange(response.Messages).

    Console.WriteLine($"\nAI: {response.Text}");
    Console.WriteLine($"Tokens used: {response.Usage?.TotalTokenCount} (In: {response.Usage?.InputTokenCount}, Out: {response.Usage?.OutputTokenCount})\n"); // printing token usage instead of logging for demonstration purposes.
}