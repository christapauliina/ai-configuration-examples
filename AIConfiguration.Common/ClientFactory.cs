using System.ClientModel; // Wraps the raw API key in an ApiKeyCredential instance required by the OpenAI client.
using DotNetEnv;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;

namespace AIConfiguration.Common;

public static class ClientFactory
{
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
    
}

