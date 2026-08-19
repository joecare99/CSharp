using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.HostCheck.InternetInfo;

internal static class Program
{
    private static Func<HttpClient> HttpClientFactory = static () => new();

    private static async Task<int> Main(string[] args)
    {
        using HttpClient httpClient = HttpClientFactory();
        return await RunAsync(args, httpClient);
    }

    internal static async Task<int> RunAsync(string[] args, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(httpClient);
        string source = args.Length > 0 ? args[0] : "wikipedia";
        string query = args.Length > 1 ? string.Join(" ", args, 1, args.Length - 1) : "C Sharp (programming language)";

        Console.WriteLine("== Internet Info HostCheck ==");
        Console.WriteLine($"Source: {source}");
        Console.WriteLine($"Query: {query}");
        Console.WriteLine();

        try
        {
            string content = await FetchAsync(httpClient, source, query);
            Console.WriteLine("Fetched content preview:");
            Console.WriteLine(content.Length > 600 ? content[..600] : content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fetch failed: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Malformed input/output checks:");
        await RunMalformedChecksAsync(httpClient);
        return 0;
    }

    private static async Task<string> FetchAsync(HttpClient httpClient, string source, string query)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("source must not be empty.", nameof(source));
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("query must not be empty.", nameof(query));
        }

        string normalizedSource = source.ToLowerInvariant();
        if (normalizedSource == "wikipedia")
        {
            return await FetchWikipediaSummaryAsync(httpClient, query);
        }

        if (normalizedSource == "rosettacode")
        {
            return await FetchTextAsync(httpClient, $"https://rosettacode.org/wiki/{Uri.EscapeDataString(query)}");
        }

        if (normalizedSource == "mslearn")
        {
            return await FetchTextAsync(httpClient, $"https://learn.microsoft.com/en-us/search/?terms={Uri.EscapeDataString(query)}");
        }

        throw new InvalidOperationException($"Source '{source}' is not allowed.");
    }

    private static async Task<string> FetchWikipediaSummaryAsync(HttpClient httpClient, string query)
    {
        string url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(query)}";
        string json = await FetchTextAsync(httpClient, url);
        using JsonDocument document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("extract", out JsonElement extract))
        {
            throw new InvalidOperationException("Wikipedia response did not contain 'extract'.");
        }

        return extract.GetString() ?? string.Empty;
    }

    private static async Task<string> FetchTextAsync(HttpClient httpClient, string url)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd("OllamaCodingAgentHostCheck/1.0 (+https://github.com)");
        request.Headers.Accept.ParseAdd("text/plain, application/json, text/html");
        using HttpResponseMessage response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private static async Task RunMalformedChecksAsync(HttpClient httpClient)
    {
        await TryMalformedCaseAsync(async () => await FetchAsync(httpClient, string.Empty, "C#"), "Empty source");
        await TryMalformedCaseAsync(async () => await FetchAsync(httpClient, "unknown", "C#"), "Unknown source");
        await TryMalformedCaseAsync(async () => await FetchAsync(httpClient, "wikipedia", string.Empty), "Empty query");
        await TryMalformedCaseAsync(async () =>
        {
            // Simulated malformed output case by trying to parse non-JSON as wiki summary.
            string html = await FetchTextAsync(httpClient, "https://example.com");
            using JsonDocument document = JsonDocument.Parse(html);
            _ = document.RootElement.ValueKind;
        }, "Malformed output parse");
    }

    private static async Task TryMalformedCaseAsync(Func<Task> action, string label)
    {
        try
        {
            await action();
            Console.WriteLine($"Unexpected success: {label}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected failure ({label}): {ex.GetType().Name}");
        }
    }
}
