using System.Text.RegularExpressions;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Removes credential-bearing user information from Git URLs before it reaches an operator-facing result.
/// </summary>
public static class GitDiagnosticRedactor
{
    private static readonly Regex HttpUserInfoPattern = new(
        @"(?<scheme>https?://)[^/\s@]+@",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex ScpUserInfoPattern = new(
        @"(?<prefix>(?:^|\s))[^@\s/:]+(?::[^@\s/]+)?@(?=[^:\s/]+:)",
        RegexOptions.CultureInvariant);
    private static readonly Regex QueryValuePattern = new(
        @"(?<prefix>[?&][^=\s]+)=[^&\s]*",
        RegexOptions.CultureInvariant);

    /// <summary>
    /// Redacts HTTP(S) and SCP-style URL user information from a Git diagnostic.
    /// </summary>
    public static string Redact(string? diagnostic)
    {
        if (string.IsNullOrEmpty(diagnostic))
        {
            return string.Empty;
        }

        string redactedDiagnostic = HttpUserInfoPattern.Replace(diagnostic, "${scheme}");
        redactedDiagnostic = ScpUserInfoPattern.Replace(redactedDiagnostic, "${prefix}");
        return QueryValuePattern.Replace(redactedDiagnostic, "${prefix}[REDACTED]");
    }
}
