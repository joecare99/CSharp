using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Protocol.Models;
using Ollama.Protocol.Services;
using Ollama.Protocol.Models;

namespace Ollama.Samples.TagsCheck;

internal static class Program
{
    private const string DefaultEndpoint = "http://localhost:11434/";

    internal static Func<HttpClient> HttpClientFactory { get; set; } = CreateHttpClient;

    internal static HttpClient CreateHttpClient() => new()
    {
        Timeout = Timeout.InfiniteTimeSpan,
    };

    internal static async Task<int> Main()
    {
        string endpointValue = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? DefaultEndpoint;

        using HttpClient httpClient = HttpClientFactory();
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri(endpointValue)));

        try
        {
            OllamaTagsResponse response = await client.GetTagsAsync();

            Console.WriteLine($"Endpoint: {endpointValue}");
            Console.WriteLine($"Model count: {response.Models.Count}");
            foreach (OllamaModelInfo model in response.Models)
            {
                Console.WriteLine($"- {model.Name ?? "<unknown>"} ({model.Model ?? "<unknown>"})");
            }

            if (!response.Models.Any())
            {
                Console.WriteLine("No models were returned by Ollama.");
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Tags check failed.");
            Console.WriteLine(ex.Message);
            return 1;
        }
    }
}
