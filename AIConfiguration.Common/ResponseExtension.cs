using Microsoft.Extensions.AI;

namespace AIConfiguration.Common
{
    public class ResponseExtension
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
                    case ToolCallContent call:
                        Console.WriteLine($"[Tool Call] {call.GetType().Name}");
                        break;
                    case TextContent text when text.Annotations is { Count: > 0 }:
                        foreach (var citation in text.Annotations.OfType<CitationAnnotation>())
                        {
                            Console.WriteLine($"[Source] {citation.Title} -> {citation.Url}");
                        }
                        break;
                }
            }
     
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