/*
===========================================================================================================================
Connecting to a stateless model, no memory of previous interactions. Each prompt is independent of the previous ones.
Docs: https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/01.model-memory
 
Test: Tell the model your name and ask it to remember it. Then ask it for your name again. 

Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
===========================================================================================================================
*/
using AIConfiguration.Common;
using Microsoft.Extensions.AI;

// Using Chat Completions API defined in ClientFactory
IChatClient client = ClientFactory.Create();

Console.WriteLine("01. Stateless model Demo");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    ChatResponse response = await client.GetResponseAsync(prompt!); // notice the parameter is just a string -> this is only valid if every prompt is an independent interaction

    Console.WriteLine($"\nAI: {response.Text}");
    Console.WriteLine($"Tokens used: {response.Usage?.TotalTokenCount} (In: {response.Usage?.InputTokenCount}, Out: {response.Usage?.OutputTokenCount})\n"); // printing token usage instead of logging for demonstration purposes. Next demos will use AIConfiguration.Common.ResponseExtension.PrintTokenUsage to print token usage.
}
    