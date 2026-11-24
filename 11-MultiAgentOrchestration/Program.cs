/*
===========================================================================================================================
Creating a simple multiagent orchestration workflow with three agents: 1) Research Agent, 2) Analyst Agent, and 3) Writer Agent, where the Research Agent is a tool for Analyst Agent. 

Test: Enter a profession and a topic when prompted to generate a LinkedIn post written from that professional's perspective.

Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo. 
===========================================================================================================================
*/


using System.ComponentModel;
using AIConfiguration.Common;
using Microsoft.Extensions.AI;

IChatClient client = ClientFactory.CreateResponsesClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

// Defining an asynchronous method representing the Research agent, which will later be wrapped into a tool.
[Description("Searches the web for up-to-date facts and returns a concise bulleted list of verified information.")]
async Task<string> SearchFactsAsync([Description("The specific topic or question to research")] string query)
{
    //Configuring the Research agent and giving it the tool the search internet
    ChatOptions researchOptions = new()
    {
        Instructions = """
        You are a focused web researcher. Use web search to find accurate, up-to-date facts. 
        Return ONLY raw, concise bullet points with key facts and numbers. No intro or outro. 
        """,
        Tools = [new HostedWebSearchTool()]
    };

    ChatResponse researchResponse = await client.GetResponseAsync(query, researchOptions);

    // Printing execution metrics for the Research Agent
    Console.WriteLine("\nResearchAgent was invoked");
    ResponseExtension.PrintReasoning(researchResponse);
    ResponseExtension.PrintToolUsage(researchResponse);
    ResponseExtension.PrintTokenUsage(researchResponse);
    return researchResponse.Text;
}

// Creating the configuration for the Main agent, which can invoke the Research agent as a tool when needed.
ChatOptions analystOptions = new()
{
    Instructions = """
        You are a critical technical analyst. Given a profession and a topic, use the SearchFacts tool to gather up-to-date information as needed, and produce a structured analysis covering:
        1) Core role description for the profession
        2) Current trends affecting the profession
        3) Key points about the provided topic
        4) How the topic relates to and impacts the profession
        """,
    Tools = [AIFunctionFactory.Create(SearchFactsAsync)]
};

// Creating the configuration for writer agent
ChatOptions producerOptions = new()
{
    Instructions = """
        You are a technical content producer. Take the provided analysis and write a crisp, engaging LinkedIn post (max 150 words) from the perspective of a representative of that profession about the given topic.
        Include 3 bullet points and a clear takeaway.
        Do not invent new facts beyond what is provided in the analysis.
        """,
};

// asking the user for parameters
Console.WriteLine("Give me a profession");
string profession = Console.ReadLine()!;
Console.WriteLine("Give me a topic");
string topic = Console.ReadLine()!;
string userPrompt = $"Profession: {profession}, Topic: {topic}";

// Giving the parameters for analysis agent, which has an option to use research agent if needed.
ChatResponse analysisResponse = await client.GetResponseAsync(userPrompt, analystOptions);

// Printing execution metrics for the Analysis Agent
Console.WriteLine("\nAnalysisAgent has done it's work");
ResponseExtension.PrintReasoning(analysisResponse);
ResponseExtension.PrintToolUsage(analysisResponse);
ResponseExtension.PrintTokenUsage(analysisResponse);

string producerPrompt = $"Create a final post based on this technical analysis:\n\n{analysisResponse.Text}";
ChatResponse finalResponse = await client.GetResponseAsync(producerPrompt, producerOptions);

// Printing execution metrics for the Writer Agent
Console.WriteLine("\nWriter Agent has done it's work");
ResponseExtension.PrintReasoning(finalResponse);
ResponseExtension.PrintToolUsage(finalResponse);
ResponseExtension.PrintTokenUsage(finalResponse);

Console.WriteLine($"\n--- Final Output (Writer Agent) ---\n{finalResponse.Text}\n");
