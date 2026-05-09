using System;
using System.IO;
using System.Net.Mime;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Services;

namespace Ollama_Service2;

internal class Program
{
    private const string DefaultModel = "qwen3.5:9b";
    private static readonly Uri GenerateEndpoint = new("http://192.168.0.32:11434/api/generate");
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static async Task<int> Main(string[] args)
    {
        string prompt = args.Length > 0
            ? string.Join(" ", args)
            : "I am a C# developer, how can you assist me ?";

        using HttpClient httpClient = new();
        OllamaGenerateRequest request = new()
        {
            Model = DefaultModel,
            Prompt = prompt,
            Stream = true,
        };

        try
        {
            httpClient.Timeout = Timeout.InfiniteTimeSpan;

            using HttpRequestMessage httpRequest = new(HttpMethod.Post, GenerateEndpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(request, JsonSerializerOptions),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json),
            };

            using HttpResponseMessage httpResponse = await httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead);

            httpResponse.EnsureSuccessStatusCode();

            Console.WriteLine($"URL: {GenerateEndpoint}");
            Console.WriteLine($"Model: {DefaultModel}");
            Console.WriteLine();
            Console.WriteLine("Prompt:");
            Console.WriteLine(prompt);
            Console.WriteLine();
            Console.WriteLine("Thinking:");

            await using Stream responseStream = await httpResponse.Content.ReadAsStreamAsync();
            using StreamReader reader = new(responseStream, Encoding.UTF8);

            bool hasThinkingOutput = false;
            bool hasResponseOutput = false;

            while (!reader.EndOfStream)
            {
                string? line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                OllamaGenerateResponseChunk? chunk = JsonSerializer.Deserialize<OllamaGenerateResponseChunk>(line, JsonSerializerOptions);
                if (chunk is null)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(chunk.Thinking))
                {
                    Console.Write(chunk.Thinking);
                    hasThinkingOutput = true;
                }

                if (!string.IsNullOrWhiteSpace(chunk.Response))
                {
                    if (!hasResponseOutput)
                    {
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("Antwort:");
                    }

                    Console.Write(chunk.Response);
                    hasResponseOutput = true;
                }

                if (chunk.Done)
                {
                    break;
                }
            }

            if (!hasThinkingOutput)
            {
                Console.WriteLine("<keine Thinking-Ausgabe>");
            }

            if (!hasResponseOutput)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Keine Antwort vom Ollama-Service erhalten.");
            }

            Console.WriteLine();

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Aufruf an Ollama fehlgeschlagen.");
            Console.WriteLine(ex.Message);
            return 1;
        }
    }
}
