using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Searches curated local wiki entries.
/// </summary>
public sealed class LocalWikiSearchTool : IOllamaTool
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly LocalKnowledgeBaseStore _store;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalWikiSearchTool"/> class.
    /// </summary>
    public LocalWikiSearchTool(LocalKnowledgeBaseStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    /// <inheritdoc />
    public string Name => "local_wiki_search";

    /// <inheritdoc />
    public string Description => "Searches local wiki entries and returns citation-aware snippets.";

    /// <inheritdoc />
    public OllamaToolSchema Schema => new()
    {
        Summary = "Search the local wiki. maxResults defaults to the store limit and must be between 1 and 20.",
        Parameters =
        [
            new OllamaToolParameter { Name = "query", Type = "string", Description = "Required non-empty text or concepts to search.", Required = true },
            new OllamaToolParameter { Name = "maxResults", Type = "number", Description = "Optional integer result limit from 1 to 20; default 10.", Required = false },
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
            LocalWikiSearchToolInput payload = JsonSerializer.Deserialize<LocalWikiSearchToolInput>(input, JsonOptions)
                ?? throw new InvalidOperationException("Invalid JSON input.");
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Query);
            if (payload.MaxResults < 1 || payload.MaxResults > 20)
            {
                return OllamaToolValidationResult.Failure("maxResults must be between 1 and 20.");
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
        LocalWikiSearchToolInput payload = JsonSerializer.Deserialize<LocalWikiSearchToolInput>(input, JsonOptions)
            ?? throw new InvalidOperationException("Invalid JSON input.");
        int maxResults = Math.Clamp(payload.MaxResults, 1, 20);
        var results = await _store.SearchAsync(payload.Query.Trim(), maxResults, cancellationToken);
        return new OllamaToolResult
        {
            Success = true,
            Output = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }),
        };
    }
}
