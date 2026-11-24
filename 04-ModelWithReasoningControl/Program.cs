/*
===========================================================================================================================
Connecting to a model and controlling its reasoning effort
Docs: https://christapauliina.github.io/AI-configuration/llm-configuration/configurations/03.model-parameters

Test: Give this prompt to application:
    "I have a demo comparing two reasoning models—one configured with low reasoning effort and the other with high reasoning effort.
    Give me a short test prompt that produces a concise final answer, while clearly highlighting the difference between the two models in
    both their reasoning traces and reasoning token counts."
Compare the differences in the answers, reasoning and token consumption.

Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
===========================================================================================================================
*/

using AIConfiguration.Common;
using Microsoft.Extensions.AI;

// For now on, we use Responses API instead of Chat Completions API, as Respones API is required to get reasoning content from the model and supports more advanced features.
IChatClient client = ClientFactory.CreateResponsesClient();

// Configuring two different reasoning effort levels for model and configuring reasoning output to full  
var options1 = new ChatOptions
{
    Reasoning = new ReasoningOptions
    {
        Effort = ReasoningEffort.Low,
        Output = ReasoningOutput.Summary
    }
};

var options2 = new ChatOptions
{
    Reasoning = new ReasoningOptions
    {
        Effort = ReasoningEffort.High,
        Output = ReasoningOutput.Summary
    }
};

Console.WriteLine("04. Model with Reasoning Control Demo");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    // Passing the reasoning options via options, in this demo we are not using history.
    ChatResponse response = await client.GetResponseAsync(prompt!, options1);

    Console.WriteLine($"\nAI Reasoning (Low): {response.Text}");

    //First time printing the reasoning in program to showcase the code. For the remaining responses we use the reusable ResponseExtension.PrintReasoning helper.
    foreach (ChatMessage message in response.Messages)
    {
        foreach (TextReasoningContent reasoning in message.Contents.OfType<TextReasoningContent>())
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n[Reasoning]:\n{reasoning.Text}");
            Console.ResetColor();
        }
    }

    long? reasoningTokens = response.Usage?.ReasoningTokenCount;

    if (reasoningTokens is not null)
    {
        Console.WriteLine($"  └─ Reasoning tokens: {reasoningTokens}");
    }

    ResponseExtension.PrintTokenUsage(response);

    ChatResponse response2 = await client.GetResponseAsync(prompt!, options2);
    Console.WriteLine($"\nAI Reasoning (High): {response2.Text}");
    ResponseExtension.PrintReasoning(response2);
    ResponseExtension.PrintTokenUsage(response2);
}