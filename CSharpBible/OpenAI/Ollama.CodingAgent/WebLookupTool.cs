using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Fetches bounded web knowledge from trusted sources.
/// </summary>
public sealed class WebLookupTool : IOllamaTool
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;
    private readonly WebKnowledgePolicy _policy;
    private readonly Func<Uri, bool> _isCitationUriAllowed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebLookupTool"/> class.
    /// </summary>
    public WebLookupTool(
        HttpClient httpClient,
        WebKnowledgePolicy policy,
        Func<Uri, bool>? isCitationUriAllowed = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _isCitationUriAllowed = isCitationUriAllowed ?? _policy.IsAllowedCitationUri;
    }

    /// <inheritdoc />
    public string Name => "web_lookup";

    /// <inheritdoc />
    public string Description => "Looks up trusted web sources and returns citation envelope output.";

    /// <inheritdoc />
    public OllamaToolSchema Schema => new()
    {
        Summary = "Look up one query using only an allow-listed source. source must be wikipedia, rosettacode, or mslearn.",
        Parameters =
        [
            new OllamaToolParameter { Name = "source", Type = "string", Description = "Required source key: wikipedia, rosettacode, or mslearn.", Required = true },
            new OllamaToolParameter { Name = "query", Type = "string", Description = "Required non-empty lookup text; response is truncated to a bounded preview.", Required = true },
        ],
    };

    /// <inheritdoc />
    public OllamaToolValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return OllamaToolValidationResult.Failure("Input JSON is required.");
        }

        try
        {
            WebLookupToolInput payload = JsonSerializer.Deserialize<WebLookupToolInput>(input, JsonOptions)
                ?? throw new InvalidOperationException("Invalid JSON input.");
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Source);
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Query);
            if (!_policy.TryResolveSourceTemplate(payload.Source, out _))
            {
                return OllamaToolValidationResult.Failure("Source is not allowed.");
            }

            return OllamaToolValidationResult.Success();
        }
        catch (Exception ex)
        {
            return OllamaToolValidationResult.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<OllamaToolResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        WebLookupToolInput payload = JsonSerializer.Deserialize<WebLookupToolInput>(input, JsonOptions)
            ?? throw new InvalidOperationException("Invalid JSON input.");
        if (!_policy.TryResolveSourceTemplate(payload.Source, out string template))
        {
            return new OllamaToolResult { Success = false, Output = "Source is not allowed." };
        }

        string encodedQuery = Uri.EscapeDataString(payload.Query.Trim().Replace(' ', '_'));
        string url = template.Replace("{query}", encodedQuery, StringComparison.Ordinal);
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd("OllamaCodingAgent/1.0 (+https://github.com)");
        request.Headers.Accept.ParseAdd("application/json, text/plain, text/html");
        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        string raw = await response.Content.ReadAsStringAsync(cancellationToken);
        if (raw.Length > 4000)
        {
            raw = raw[..4000];
        }

        Uri citationUri = new(url);
        if (!_isCitationUriAllowed(citationUri))
        {
            return new OllamaToolResult { Success = false, Output = "Resolved citation host is not allowed." };
        }

        WebKnowledgeLookupResult result = new()
        {
            Citation = new WebKnowledgeCitation
            {
                Source = payload.Source.ToLowerInvariant(),
                Query = payload.Query.Trim(),
                Url = citationUri.AbsoluteUri,
            },
            StatusCode = (int)response.StatusCode,
            ContentPreview = raw,
        };

        return new OllamaToolResult
        {
            Success = response.IsSuccessStatusCode,
            Output = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }),
        };
    }
}
