// ============================================================================
// Connecting to a model and controlling its reasoning effort
//
// Test: Ask a model to give you a feedback from a text you provide, e.g. a short essay or a poem. The application will give you three answers with different reasoning effort, and you can compare the differences in the answers, reasoning and token consumption.
//
// Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
// ============================================================================

using AIConfiguration.Common;
using Microsoft.Extensions.AI;
using System.Runtime.Intrinsics.X86;

IChatClient client = ClientFactory.Create();

Console.WriteLine("04. Model with Reasoning Control Demo");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

// Configuring three different reasoning effort levels for model and configuring reasoning output to full  
var options1 = new ChatOptions
{
    Reasoning = new ReasoningOptions
    {
        Effort = ReasoningEffort.None,      // None, Low, Medium or High 
        Output = ReasoningOutput.Summary    // None, Summary or Full
    }

};

var options2 = new ChatOptions
{
    Reasoning = new ReasoningOptions
    {
        Effort = ReasoningEffort.Low,      
        Output = ReasoningOutput.Summary   
    }

};

var options3 = new ChatOptions
{
    Reasoning = new ReasoningOptions
    {
        Effort = ReasoningEffort.High,      
        Output = ReasoningOutput.Summary    
    }

};


while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;



    // Passing the reasoning options via options, in this demo we are not using history.
    ChatResponse response = await client.GetResponseAsync(prompt!, options1);

    Console.WriteLine($"\nAI Reasoning (None): {response.Text}");

    //First response shows the inspection logic inline; for the remaining responses we use the reusable ResponseExtension.PrintReasoning helper.
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
    Console.WriteLine($"\nAI Reasoning (Low): {response2.Text}");
    ResponseExtension.PrintReasoning(response2);
    ResponseExtension.PrintTokenUsage(response2);


    ChatResponse response3 = await client.GetResponseAsync(prompt!, options3);
    Console.WriteLine($"\nAI Reasoning (High): {response3.Text}");
    ResponseExtension.PrintReasoning(response3);
    ResponseExtension.PrintTokenUsage(response3);
}