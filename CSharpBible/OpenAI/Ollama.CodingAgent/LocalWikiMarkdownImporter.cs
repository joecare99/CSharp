using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent;

/// <summary>
/// Imports Markdown pages from a local wiki vault into the curated JSON store.
/// </summary>
public sealed class LocalWikiMarkdownImporter
{
    private static readonly Regex TitlePattern = new(
        @"^\s*#\s+(.+?)\s*$",
        RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex FrontmatterPattern = new(
        @"\A---\s*\r?\n(?<frontmatter>.*?)\r?\n---\s*\r?\n?",
        RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex FrontmatterTitlePattern = new(
        @"^title:\s*[\""']?(?<title>.*?)[\""']?\s*$",
        RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex FrontmatterTagsPattern = new(
        @"^tags:\s*\[(?<tags>.*?)\]\s*$",
        RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Imports all Markdown pages below a vault directory.
    /// </summary>
    public async Task<LocalWikiImportResult> ImportAsync(
        string vaultDirectory,
        LocalKnowledgeBaseStore store,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vaultDirectory);
        ArgumentNullException.ThrowIfNull(store);
        string root = Path.GetFullPath(vaultDirectory);
        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException($"Wiki vault '{root}' was not found.");
        }

        int imported = 0;
        int skipped = 0;
        foreach (string filePath in Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            string markdown = await File.ReadAllTextAsync(filePath, cancellationToken);
            if (string.IsNullOrWhiteSpace(markdown))
            {
                skipped++;
                continue;
            }

            string relativePath = Path.GetRelativePath(root, filePath);
            Match frontmatterMatch = FrontmatterPattern.Match(markdown);
            string frontmatter = frontmatterMatch.Success
                ? frontmatterMatch.Groups["frontmatter"].Value
                : string.Empty;
            string content = frontmatterMatch.Success
                ? markdown[frontmatterMatch.Length..]
                : markdown;
            string title = FrontmatterTitlePattern.Match(frontmatter).Groups["title"].Value.Trim();
            if (title.Length >= 2 && title[0] == title[^1] && (title[0] == '"' || title[0] == '\''))
            {
                title = title[1..^1];
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                title = TitlePattern.Match(content).Groups[1].Value.Trim();
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                title = Path.GetFileNameWithoutExtension(filePath);
            }

            string summary = content.Trim();
            if (summary.Length > 8000)
            {
                summary = summary[..8000];
            }

            List<string> tags = relativePath
                .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
                .SkipLast(1)
                .Where(segment => !segment.StartsWith(".", StringComparison.Ordinal))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            Match tagsMatch = FrontmatterTagsPattern.Match(frontmatter);
            if (tagsMatch.Success)
            {
                tags.AddRange(tagsMatch.Groups["tags"].Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(tag => tag.Trim(' ', '\'', '"'))
                    .Where(tag => tag.Length > 0));
            }

            await store.AddOrUpdateAsync(new LocalKnowledgeEntry
            {
                Id = relativePath.Replace(Path.DirectorySeparatorChar, '/'),
                Title = title,
                Summary = summary,
                Source = "codewikivault",
                Tags = tags.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            }, cancellationToken);
            imported++;
        }

        return new LocalWikiImportResult
        {
            ImportedCount = imported,
            SkippedCount = skipped,
        };
    }
}
