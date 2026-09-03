using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SelfImprovingHarness;
public interface IOllamaClient { Task<string> GenerateAsync(string prompt, CancellationToken ct = default); }
public sealed class OllamaClient(HttpClient http, IOptions<OllamaOptions> options) : IOllamaClient
{
    private readonly OllamaOptions _options = options.Value;
    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        var payload = new { model = _options.Model, prompt, stream = false };
        Exception? last = null;
        for (var attempt = 0; attempt <= _options.MaxRetries; attempt++)
        {
            try { using var response = await http.PostAsJsonAsync("/api/generate", payload, ct); response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
                return body.TryGetProperty("response", out var text) ? text.GetString() ?? "" : throw new InvalidDataException("Ollama response lacks 'response'."); }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidDataException) { last = ex; if (attempt < _options.MaxRetries) await Task.Delay(TimeSpan.FromMilliseconds(250 * (attempt + 1)), ct); }
        }
        throw new IOException($"Ollama unavailable after {_options.MaxRetries + 1} attempts: {last?.Message}", last);
    }
}
