// Core/OllamaClient.cs
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OllamaHarness.Core;

public sealed class OllamaClient(HttpClient http, string baseUrl, string model)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<string> GenerateAsync(
        string prompt,
        string system = "",
        double temperature = 0.2,
        CancellationToken ct = default)
    {
        var request = new OllamaRequest
        {
            Model = model,
            Prompt = prompt,
            System = system,
            Stream = false,
            Options = new OllamaOptions { Temperature = temperature }
        };

        var response = await http.PostAsJsonAsync(
            $"{baseUrl}/api/generate", request, JsonOpts, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaResponse>(JsonOpts, ct);

        return result?.Response ?? throw new InvalidOperationException("Empty Ollama response.");
    }

    public async Task<string> ChatAsync(
        List<ChatMessage> messages,
        double temperature = 0.2,
        CancellationToken ct = default)
    {
        var request = new OllamaChatRequest
        {
            Model = model,
            Messages = messages,
            Stream = false,
            Options = new OllamaOptions { Temperature = temperature }
        };

        var response = await http.PostAsJsonAsync(
            $"{baseUrl}/api/chat", request, JsonOpts, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaChatResponse>(JsonOpts, ct);

        return result?.Message?.Content
            ?? throw new InvalidOperationException("Empty Ollama chat response.");
    }

    public async Task<bool> IsAliveAsync(CancellationToken ct = default)
    {
        try
        {
            var resp = await http.GetAsync($"{baseUrl}/api/tags", ct);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}

// --- DTOs ---
public sealed record OllamaRequest
{
    public required string Model { get; init; }
    public required string Prompt { get; init; }
    public string System { get; init; } = "";
    public bool Stream { get; init; }
    public OllamaOptions? Options { get; init; }
}

public sealed record OllamaOptions
{
    public double Temperature { get; init; }
    public int NumPredict { get; init; } = 4096;
}

public sealed record OllamaResponse
{
    public string? Response { get; init; }
}

public sealed record OllamaChatRequest
{
    public required string Model { get; init; }
    public required List<ChatMessage> Messages { get; init; }
    public bool Stream { get; init; }
    public OllamaOptions? Options { get; init; }
}

public sealed record ChatMessage
{
    public required string Role { get; init; }
    public required string Content { get; init; }
}

public sealed record OllamaChatResponse
{
    public ChatMessage? Message { get; init; }
}