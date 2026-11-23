using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
    }
}