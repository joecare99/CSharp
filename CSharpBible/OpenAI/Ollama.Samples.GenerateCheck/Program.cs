using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Protocol;
using Ollama.Protocol.Models;

namespace Ollama.Samples.GenerateCheck;

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
            : "Explain AI in one sentence.";

        using HttpClient httpClient = new()
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri(endpointValue)));

        try
        {
            Console.WriteLine($"Endpoint: {endpointValue}");
            Console.WriteLine($"Model: {model}");
            Console.WriteLine();
            Console.WriteLine("Prompt:");
            Console.WriteLine(prompt);
            Console.WriteLine();
            Console.WriteLine("Thinking:");

            bool hasThinking = false;
            bool hasAnswer = false;

            await foreach (OllamaGenerateResponseChunk chunk in client.GenerateStreamingAsync(new OllamaGenerateRequest
            {
                Model = model,
                Prompt = prompt,
                Stream = true,
            }))
            {
                if (!string.IsNullOrWhiteSpace(chunk.Thinking))
                {
                    Console.Write(chunk.Thinking);
                    hasThinking = true;
                }

                if (!string.IsNullOrWhiteSpace(chunk.Response))
                {
                    if (!hasAnswer)
                    {
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("Answer:");
                    }

                    Console.Write(chunk.Response);
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
            Console.WriteLine("Generate check failed.");
            Console.WriteLine(ex.Message);
            return 1;
        }
    }
}
