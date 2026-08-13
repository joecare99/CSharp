using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.Tools;

/// <summary>
/// Stores bounded session memory in a local JSON file.
/// </summary>
public sealed class JsonSessionMemoryStore : ISessionMemoryStore
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "the", "and", "are", "for", "from", "that", "this", "which", "what", "with", "uses", "used",
    };
    private readonly string _filePath;
    private readonly int _maximumEntriesPerSession;
    private readonly SemaphoreSlim _gate = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSessionMemoryStore"/> class.
    /// </summary>
    public JsonSessionMemoryStore(string filePath, int maximumEntriesPerSession = 100)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (maximumEntriesPerSession <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumEntriesPerSession));
        }

        _filePath = Path.GetFullPath(filePath);
        _maximumEntriesPerSession = maximumEntriesPerSession;
    }

    /// <inheritdoc />
    public async Task RememberAsync(
        string sessionId,
        string content,
        CancellationToken cancellationToken = default)
    {
        ValidateInput(sessionId, content);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            List<SessionMemoryEntry> entries = await ReadAsync(cancellationToken);
            entries.Add(new SessionMemoryEntry { SessionId = sessionId, Content = content });
            entries = entries
                .Where(entry => !string.Equals(entry.SessionId, sessionId, StringComparison.Ordinal))
                .Concat(entries
                    .Where(entry => string.Equals(entry.SessionId, sessionId, StringComparison.Ordinal))
                    .TakeLast(_maximumEntriesPerSession))
                .ToList();
            await WriteAsync(entries, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SessionMemoryEntry>> RecallAsync(
        string sessionId,
        string query,
        int maximumResults = 5,
        CancellationToken cancellationToken = default)
    {
        ValidateInput(sessionId, query);
        if (maximumResults <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumResults));
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            IReadOnlyList<SessionMemoryEntry> entries = (await ReadAsync(cancellationToken))
                .Where(entry => string.Equals(entry.SessionId, sessionId, StringComparison.Ordinal))
                .ToArray();
            string[] queryTerms = GetTerms(query);
            return entries
                .Select(entry => new
                {
                    Entry = entry,
                    Score = GetTerms(entry.Content).Intersect(queryTerms, StringComparer.OrdinalIgnoreCase).Count(),
                })
                .Where(item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenByDescending(item => item.Entry.CreatedAt)
                .Take(maximumResults)
                .Select(item => item.Entry)
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<SessionMemoryEntry>> ReadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using FileStream stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<SessionMemoryEntry>>(stream, cancellationToken: cancellationToken)
            ?? [];
    }

    private async Task WriteAsync(List<SessionMemoryEntry> entries, CancellationToken cancellationToken)
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, entries, cancellationToken: cancellationToken);
    }

    private static string[] GetTerms(string value) => value
        .Split([' ', '\t', '\r', '\n', '.', ',', ':', ';', '/', '\\', '-', '_'], StringSplitOptions.RemoveEmptyEntries)
        .Where(term => term.Length >= 2)
        .Where(term => !StopWords.Contains(term))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private static void ValidateInput(string sessionId, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
    }
}
