using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Protocol;
using Ollama.Protocol.Models;

namespace Ollama.Samples.EmbedCheck;

internal static class Program
{
    private const string DefaultEndpoint = "http://localhost:11434/";
    private const string DefaultModel = "nomic-embed-text";

    private static async Task<int> Main(string[] args)
    {
        string endpointValue = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? DefaultEndpoint;
        string model = Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? DefaultModel;
        string input = args.Length > 0
            ? string.Join(" ", args)
            : "Artificial intelligence helps developers automate tasks.";

        using HttpClient httpClient = new()
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri(endpointValue)));

        try
        {
            OllamaEmbedResponse response = await client.EmbedAsync(new OllamaEmbedRequest
            {
                Model = model,
                Input = [input],
            });

            Console.WriteLine($"Endpoint: {endpointValue}");
            Console.WriteLine($"Model: {model}");
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Vector count: {response.Embeddings.Count}");
            Console.WriteLine($"First vector length: {(response.Embeddings.Count > 0 ? response.Embeddings[0].Count : 0)}");

            if (response.Embeddings.Count > 0 && response.Embeddings[0].Count > 0)
            {
                Console.WriteLine($"First value: {response.Embeddings[0][0]}");
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Embed check failed.");
            Console.WriteLine(ex.Message);
            return 1;
        }
    }
}
