using Microsoft.Extensions.AI;

namespace AIConfiguration.Common
{
    public static class ResponseExtension
    {
        public static void PrintTokenUsage(ChatResponse response)
        {
            Console.WriteLine($"Tokens used: {response.Usage?.TotalTokenCount} (In: {response.Usage?.InputTokenCount}, Out: {response.Usage?.OutputTokenCount})\n");
        }

        public static void PrintToolUsage(ChatResponse response)
        {
            foreach (var content in response.Messages.SelectMany(m => m.Contents))
            {
                switch (content)
                {
                    case FunctionCallContent function:
                        // Local functions and MCP tools: show the tool name and the arguments the model chose.
                        Console.WriteLine($"[Tool Call] {function.Name}({FormatArguments(function.Arguments)})");
                        break;
                    case ToolCallContent call:
                        // Hosted tools (e.g. web search) don't expose a function name, so show the type and call id.
                        Console.WriteLine($"[Tool Call] {call.GetType().Name} (CallId: {call.CallId})");
                        break;
                }
            }
        }

        private static string FormatArguments(IDictionary<string, object?>? arguments)
        {
            if (arguments is null || arguments.Count == 0)
                return string.Empty;
            // Long values (e.g. DAX queries) are truncated to keep the console output readable.
            return string.Join(", ", arguments.Select(a =>
            {
                string value = a.Value?.ToString() ?? "null";
                if (value.Length > 80)
                    value = value[..80] + "...";
                return $"{a.Key}={value}";
            }));
        }


        public static void PrintReasoning(ChatResponse response)
        {
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
        }
    }
}