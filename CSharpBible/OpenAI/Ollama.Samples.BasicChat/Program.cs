using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Client.Models;

namespace Ollama.Samples.BasicChat;

internal static class Program
{
    private const string DefaultEndpoint = "http://localhost:11434/";
    private const string DefaultModel = "qwen3.5:4b";

    private static async Task<int> Main(string[] args)
    {
        string endpointValue = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? DefaultEndpoint;
        string model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? DefaultModel;
        string prompt = args.Length > 0
            ? string.Join(" ", args)
            : "Explain artificial intelligence in one short sentence.";

        using HttpClient httpClient = new()
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri(endpointValue)));
        OllamaChatClient chatClient = client.GetChatClient(model);

        try
        {
            Console.WriteLine($"Endpoint: {endpointValue}");
            Console.WriteLine($"Requested model: {model}");
            Console.WriteLine();
            Console.WriteLine("Prompt:");
            Console.WriteLine(prompt);
            Console.WriteLine();
            Console.WriteLine("Thinking:");

            bool hasThinking = false;
            bool hasAnswer = false;
            ChatCompletionOptions options = new()
            {
                Messages = new List<OllamaClientChatMessage>
                {
                    new()
                    {
                        Role = "system",
                        Content = "Answer briefly and clearly.",
                    },
                    new()
                    {
                        Role = "user",
                        Content = prompt,
                    },
                },
            };

            await foreach (OllamaStreamingChatUpdate chunk in chatClient.CompleteChatStreamingAsync(options))
            {
                if (!string.IsNullOrWhiteSpace(chunk.Thinking))
                {
                    Console.Write(chunk.Thinking);
                    hasThinking = true;
                }

                if (!string.IsNullOrWhiteSpace(chunk.Content))
                {
                    if (!hasAnswer)
                    {
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("Answer:");
                    }

                    Console.Write(chunk.Content);
                    hasAnswer = true;
                }
            }

            if (!hasThinking)
            {
                Console.WriteLine("<no thinking output>");
            }

            if (!hasAnswer)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("<no answer output>");
            }

            Console.WriteLine();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("The Ollama sample failed.");
            Console.WriteLine(ex.Message);
            return 1;
        }
    }
}
