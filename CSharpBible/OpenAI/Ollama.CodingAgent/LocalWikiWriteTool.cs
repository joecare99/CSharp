using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Writes curated local wiki entries.
/// </summary>
public sealed class LocalWikiWriteTool : IOllamaTool
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly LocalKnowledgeBaseStore _store;
    private readonly LocalWikiWritePolicy _writePolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalWikiWriteTool"/> class.
    /// </summary>
    public LocalWikiWriteTool(LocalKnowledgeBaseStore store, LocalWikiWritePolicy? writePolicy = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _writePolicy = writePolicy ?? new LocalWikiWritePolicy();
    }

    /// <inheritdoc />
    public string Name => "local_wiki_write";

    /// <inheritdoc />
    public string Description => "Writes one curated local wiki entry with tags and optional citation URL.";

    /// <inheritdoc />
    public OllamaToolSchema Schema => new()
    {
        Summary = "Create or update one local wiki entry. id, title, and summary are required; summary is capped at 8000 characters.",
        Parameters =
        [
            new OllamaToolParameter { Name = "id", Type = "string", Description = "Required stable identifier used for upsert.", Required = true },
            new OllamaToolParameter { Name = "title", Type = "string", Description = "Required human-readable title.", Required = true },
            new OllamaToolParameter { Name = "summary", Type = "string", Description = "Required curated summary, maximum 8000 characters.", Required = true },
            new OllamaToolParameter { Name = "source", Type = "string", Description = "Optional source label; defaults to 'local'.", Required = false },
            new OllamaToolParameter { Name = "citationUrl", Type = "string", Description = "Optional absolute citation URL.", Required = false },
            new OllamaToolParameter { Name = "tags", Type = "array", Description = "Optional string array of topic tags; blank tags are ignored.", Required = false },
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
            LocalWikiWriteToolInput payload = JsonSerializer.Deserialize<LocalWikiWriteToolInput>(input, JsonOptions)
                ?? throw new InvalidOperationException("Invalid JSON input.");
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Id);
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Title);
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Summary);
            if (payload.Summary.Length > 8000)
            {
                return OllamaToolValidationResult.Failure("summary is too large (max 8000 chars).");
            }

            if (!string.IsNullOrWhiteSpace(payload.CitationUrl))
            {
                if (!Uri.TryCreate(payload.CitationUrl, UriKind.Absolute, out Uri? citationUri))
                {
                    return OllamaToolValidationResult.Failure("citationUrl must be an absolute URL.");
                }

                if (!_writePolicy.TryValidate(new LocalKnowledgeEntry
                    {
                        Id = payload.Id,
                        Title = payload.Title,
                        Summary = payload.Summary,
                        CitationUrl = citationUri.AbsoluteUri,
                    }, out string citationError))
                {
                    return OllamaToolValidationResult.Failure(citationError);
                }
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
        LocalWikiWriteToolInput payload = JsonSerializer.Deserialize<LocalWikiWriteToolInput>(input, JsonOptions)
            ?? throw new InvalidOperationException("Invalid JSON input.");

        LocalKnowledgeEntry entry = new()
        {
            Id = payload.Id.Trim(),
            Title = payload.Title.Trim(),
            Summary = payload.Summary.Trim(),
            Source = string.IsNullOrWhiteSpace(payload.Source) ? "local" : payload.Source.Trim(),
            CitationUrl = string.IsNullOrWhiteSpace(payload.CitationUrl) ? null : payload.CitationUrl.Trim(),
            Tags = payload.Tags.Where(tag => !string.IsNullOrWhiteSpace(tag)).Select(tag => tag.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
        };
        await _store.AddOrUpdateAsync(entry, cancellationToken);

        string output = JsonSerializer.Serialize(new
        {
            status = "ok",
            id = entry.Id,
            title = entry.Title,
            source = entry.Source,
            citationUrl = entry.CitationUrl,
            tags = entry.Tags,
        }, new JsonSerializerOptions { WriteIndented = true });

        return new OllamaToolResult { Success = true, Output = output };
    }
}
