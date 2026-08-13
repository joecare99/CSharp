using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent;

/// <summary>
/// Provides file-based storage and lookup for a local knowledge base.
/// </summary>
public sealed class LocalKnowledgeBaseStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _databaseFilePath;
    private readonly LocalWikiWritePolicy _writePolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalKnowledgeBaseStore"/> class.
    /// </summary>
    /// <param name="databaseFilePath">The JSON database file path.</param>
    public LocalKnowledgeBaseStore(string databaseFilePath, LocalWikiWritePolicy? writePolicy = null)
    {
        _databaseFilePath = string.IsNullOrWhiteSpace(databaseFilePath)
            ? throw new ArgumentException("Database file path must not be empty.", nameof(databaseFilePath))
            : Path.GetFullPath(databaseFilePath);
        _writePolicy = writePolicy ?? new LocalWikiWritePolicy();
    }

    /// <summary>
    /// Gets the database file path.
    /// </summary>
    public string DatabaseFilePath => _databaseFilePath;

    /// <summary>
    /// Adds or updates one entry in the local knowledge base.
    /// </summary>
    public async Task AddOrUpdateAsync(LocalKnowledgeEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ValidateEntry(entry);

        List<LocalKnowledgeEntry> entries = await LoadEntriesAsync(cancellationToken);
        int existingIndex = entries.FindIndex(candidate => string.Equals(candidate.Id, entry.Id, StringComparison.OrdinalIgnoreCase));
        if (existingIndex >= 0)
        {
            entries[existingIndex] = entry;
        }
        else
        {
            entries.Add(entry);
        }

        await SaveEntriesAsync(entries, cancellationToken);
    }

    /// <summary>
    /// Searches entries by query text.
    /// </summary>
    public async Task<IReadOnlyList<LocalKnowledgeEntry>> SearchAsync(string query, int maxResults = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query must not be empty.", nameof(query));
        }

        if (maxResults < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxResults), "maxResults must be >= 1.");
        }

        List<LocalKnowledgeEntry> entries = await LoadEntriesAsync(cancellationToken);
        string normalized = query.Trim();
        string[] queryTerms = GetTerms(normalized);
        return entries
            .Select(entry => new
            {
                Entry = entry,
                Score = CalculateRelevance(entry, normalized, queryTerms),
            })
            .Where(item => item.Score > 0)
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Entry.Id, StringComparer.OrdinalIgnoreCase)
            .Take(maxResults)
            .Select(item => item.Entry)
            .ToArray();
    }

    /// <summary>
    /// Loads all entries.
    /// </summary>
    public async Task<IReadOnlyList<LocalKnowledgeEntry>> LoadAllAsync(CancellationToken cancellationToken = default)
    {
        return await LoadEntriesAsync(cancellationToken);
    }

    private async Task<List<LocalKnowledgeEntry>> LoadEntriesAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_databaseFilePath))
        {
            return [];
        }

        string json = await File.ReadAllTextAsync(_databaseFilePath, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        List<LocalKnowledgeEntry>? entries = JsonSerializer.Deserialize<List<LocalKnowledgeEntry>>(json, JsonOptions);
        if (entries is null)
        {
            return [];
        }

        foreach (LocalKnowledgeEntry entry in entries)
        {
            ValidateEntry(entry);
        }

        return entries;
    }

    private async Task SaveEntriesAsync(IReadOnlyList<LocalKnowledgeEntry> entries, CancellationToken cancellationToken)
    {
        string directoryPath = Path.GetDirectoryName(_databaseFilePath) ?? ".";
        Directory.CreateDirectory(directoryPath);
        string json = JsonSerializer.Serialize(entries, JsonOptions);
        await File.WriteAllTextAsync(_databaseFilePath, json, cancellationToken);
    }

    private void ValidateEntry(LocalKnowledgeEntry entry)
    {
        if (!_writePolicy.TryValidate(entry, out string error))
        {
            throw new ArgumentException(error, nameof(entry));
        }
    }

    private static int CalculateRelevance(LocalKnowledgeEntry entry, string query, IReadOnlyCollection<string> queryTerms)
    {
        int score = 0;
        if (entry.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            score += 10;
        }

        if (entry.Summary.Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            score += 3;
        }

        foreach (string term in queryTerms)
        {
            if (entry.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score += 5;
            }

            if (entry.Tags.Any(tag => tag.Contains(term, StringComparison.OrdinalIgnoreCase)))
            {
                score += 3;
            }

            if (entry.Summary.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score++;
            }
        }

        return score;
    }

    private static string[] GetTerms(string value) => value
        .Split([' ', '\t', '\r', '\n', '.', ',', ':', ';', '/', '\\', '-', '_'], StringSplitOptions.RemoveEmptyEntries)
        .Where(term => term.Length >= 2)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
}
