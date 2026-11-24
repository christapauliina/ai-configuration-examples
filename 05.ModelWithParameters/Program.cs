/*
===========================================================================================================================
Connecting to a model and configuring it's behavior via parameters.

Test: Give the model a creative but open prompt, for example give three topics for short stories and a example title for each. Give the same prompt twice.
Temperature, TopP,: the deterministic model may give almost same answer in both times as the creative model may give two wildy different answers.
MaxOutputTokens: the deterministic model may give a shorter answer or cut it's answer off. 
Also notice the difference in token consumption between the two configurations.

Note1: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
Note2: Usually you would configure only Temperature or TopP, not both.
===========================================================================================================================
*/
using AIConfiguration.Common;
using Microsoft.Extensions.AI;

IChatClient client = ClientFactory.CreateResponsesClient();

// Configuration 1: Deterministic and concise responses
var deterministicOptions = new ChatOptions
{
    Temperature = 0.0f,
    TopP = 0.1f,
    MaxOutputTokens = 100,
    Reasoning = new ReasoningOptions { Effort = ReasoningEffort.None }     // Required if running on a reasoning model like GPT-5.2, otherwise Temperature/TopP may be rejected
};

// Configuration 2: Creative and more varied vocabulary
var creativeOptions = new ChatOptions
{
    Temperature = 1.3f,
    TopP = 0.98f,
    MaxOutputTokens = 250,
    Reasoning = new ReasoningOptions { Effort = ReasoningEffort.None }
};

Console.WriteLine("05. Model with Parameters Demo");
Console.WriteLine("Enter a prompt or 'quit' to exit:\n");

while (true)
{
    Console.WriteLine("You:");
    string? prompt = Console.ReadLine();

    if (prompt == "quit")
        break;

    ChatResponse deterministicResponse = await client.GetResponseAsync(prompt!, deterministicOptions);
    Console.WriteLine($"\n Deterministic AI:\n{deterministicResponse.Text}");
    ResponseExtension.PrintTokenUsage(deterministicResponse);

    ChatResponse creativeResponse = await client.GetResponseAsync(prompt!, creativeOptions);
    Console.WriteLine($"\n Creative AI:\n{creativeResponse.Text}");
    ResponseExtension.PrintTokenUsage(creativeResponse);
}