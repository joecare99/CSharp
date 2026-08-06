using System;
using System.Net;
using System.Text.RegularExpressions;

namespace RnzTrauer.Import.Services;

/// <summary>
/// Portable replacement for the legacy <c>HTML2text</c> helper.
/// It intentionally performs text normalization only; schema matching remains a separate concern.
/// </summary>
public sealed class HtmlTextNormalizer : IHtmlTextNormalizer
{
    private static readonly Regex ScriptStylePattern = new(
        @"<\s*(script|style)\b[^>]*>.*?<\s*/\s*\1\s*>",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
    private static readonly Regex TagPattern = new(
        @"<[^>]*>",
        RegexOptions.Singleline | RegexOptions.CultureInvariant);
    private static readonly Regex WhitespacePattern = new(
        @"\s+",
        RegexOptions.CultureInvariant);

    /// <inheritdoc />
    public string Normalize(string html)
    {
        html ??= string.Empty;
        var text = ScriptStylePattern.Replace(html, " ");
        text = TagPattern.Replace(text, " ");
        text = WebUtility.HtmlDecode(text);
        text = text.Replace("\u00a0", " ", StringComparison.Ordinal)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);
        return WhitespacePattern.Replace(text, " ").Trim();
    }
}
