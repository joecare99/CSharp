using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace Ollama.CodingAgent;

/// <summary>
/// Redacts credentials from LLM traffic before it reaches a log sink.
/// </summary>
internal static partial class LlmTrafficRedactor
{
    private const string RedactedValue = "[REDACTED]";

    /// <summary>
    /// Redacts known credentials from arbitrary diagnostic text.
    /// </summary>
    public static string Redact(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        string redacted = CredentialPropertyRegex().Replace(value, "$1" + RedactedValue + "$3");
        redacted = BearerTokenRegex().Replace(redacted, "Bearer " + RedactedValue);
        return UrlCredentialRegex().Replace(redacted, "$1" + RedactedValue + "@");
    }

    [GeneratedRegex("(\\\"?(?:authorization|api[_-]?key|access[_-]?token|refresh[_-]?token|password|secret)\\\"?\\s*:\\s*\\\")([^\\\"]*)(\\\")", RegexOptions.IgnoreCase)]
    private static partial Regex CredentialPropertyRegex();

    [GeneratedRegex("\\bBearer\\s+[^\\s,\\\"]+", RegexOptions.IgnoreCase)]
    private static partial Regex BearerTokenRegex();

    [GeneratedRegex("(https?://)([^/\\s@]+)@", RegexOptions.IgnoreCase)]
    private static partial Regex UrlCredentialRegex();
}
