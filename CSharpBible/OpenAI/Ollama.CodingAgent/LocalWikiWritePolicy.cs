using System;

namespace Ollama.CodingAgent;

/// <summary>
/// Enforces consistency and citation rules for local wiki entries.
/// </summary>
public sealed class LocalWikiWritePolicy
{
    private readonly WebKnowledgePolicy _webKnowledgePolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalWikiWritePolicy"/> class.
    /// </summary>
    public LocalWikiWritePolicy(WebKnowledgePolicy? webKnowledgePolicy = null)
    {
        _webKnowledgePolicy = webKnowledgePolicy ?? new WebKnowledgePolicy();
    }

    /// <summary>
    /// Validates one curated entry.
    /// </summary>
    public bool TryValidate(LocalKnowledgeEntry entry, out string error)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (string.IsNullOrWhiteSpace(entry.Id)
            || string.IsNullOrWhiteSpace(entry.Title)
            || string.IsNullOrWhiteSpace(entry.Summary))
        {
            error = "id, title, and summary are required.";
            return false;
        }

        if (entry.Summary.Length > 8000)
        {
            error = "summary is too large (max 8000 chars).";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(entry.CitationUrl))
        {
            if (!Uri.TryCreate(entry.CitationUrl, UriKind.Absolute, out Uri? citationUri)
                || !_webKnowledgePolicy.IsAllowedCitationUri(citationUri))
            {
                error = "citationUrl must use an allow-listed HTTPS knowledge host.";
                return false;
            }
        }

        error = string.Empty;
        return true;
    }
}
