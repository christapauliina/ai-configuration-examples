using System.ClientModel; // Wraps the raw API key in an ApiKeyCredential instance required by the OpenAI client.
using DotNetEnv;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;

namespace AIConfiguration.Common;

public static class ClientFactory
{
    /// <summary>
    /// Creates a standard chat client targeting the traditional Chat Completions API (/v1/chat/completions).
    /// Best for stateless text generation and local C# function tools without cloud-hosted server tools.
    /// </summary>
    /// <returns>An <see cref="IChatClient"/> instance configured for Chat Completions.</returns>
    public static IChatClient Create()
    {
        Env.TraversePath().Load();

        Uri endpoint = new Uri(Environment.GetEnvironmentVariable("ENDPOINT")!);
        string apiKey = Environment.GetEnvironmentVariable("API_KEY")!;
        string model = Environment.GetEnvironmentVariable("MODEL")!;

        var options = new OpenAIClientOptions { Endpoint = endpoint };

        return new OpenAIClient(new ApiKeyCredential(apiKey), options)
            .GetChatClient(model)
            .AsIChatClient(); // using IChatClient interface for abstraction and flexibility - changing platforms in the future will be easier.
    }

    // Suppress experimental API warning: OpenAI's Responses API is still marked for evaluation in the .NET SDK.
    /// <summary>
    /// Creates an agent-capable client targeting the newer Responses API (/v1/responses).
    /// Required for built-in cloud tools (e.g., HostedWebSearchTool), multi-step reasoning traces, and source citations.
    /// </summary>
    /// <returns>An <see cref="IChatClient"/> instance configured for the Responses API.</returns>
#pragma warning disable OPENAI001
    public static IChatClient CreateResponsesClient()
    {
        Env.TraversePath().Load();

        Uri endpoint = new Uri(Environment.GetEnvironmentVariable("ENDPOINT")!);
        string apiKey = Environment.GetEnvironmentVariable("API_KEY")!;
        string model = Environment.GetEnvironmentVariable("MODEL")!;

        var options = new OpenAIClientOptions { Endpoint = endpoint };

        return new OpenAIClient(new ApiKeyCredential(apiKey), options)
            .GetResponsesClient()       
            .AsIChatClient(model);     
    }
#pragma warning restore OPENAI001

}

