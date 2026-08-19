using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Defines allowed web-knowledge sources for delegated lookup.
/// </summary>
public sealed class WebKnowledgePolicy
{
    private static readonly IReadOnlyDictionary<string, string> Sources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["wikipedia"] = "https://en.wikipedia.org/api/rest_v1/page/summary/{query}",
        ["rosettacode"] = "https://rosettacode.org/wiki/{query}",
        ["mslearn"] = "https://learn.microsoft.com/api/search?search={query}&locale=en-us",
    };
    private static readonly IReadOnlySet<string> AllowedHosts = new HashSet<string>(
        ["en.wikipedia.org", "rosettacode.org", "learn.microsoft.com"],
        StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Tries to resolve one source template by source key.
    /// </summary>
    public bool TryResolveSourceTemplate(string source, out string template)
    {
        return Sources.TryGetValue(source, out template!);
    }

    /// <summary>
    /// Gets all allowed source keys.
    /// </summary>
    public IReadOnlyCollection<string> AllowedSources => (IReadOnlyCollection<string>)Sources.Keys;

    /// <summary>
    /// Validates that a citation points to an allow-listed HTTPS host.
    /// </summary>
    public bool IsAllowedCitationUri(Uri citationUri)
    {
        ArgumentNullException.ThrowIfNull(citationUri);
        return citationUri.Scheme == Uri.UriSchemeHttps
            && AllowedHosts.Contains(citationUri.Host);
    }
}
