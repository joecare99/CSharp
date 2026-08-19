using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent.Application.Services;

/// <summary>
/// Persists one agent session snapshot in a local JSON file.
/// </summary>
public sealed class JsonAgentSessionStore : IAgentSessionStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonAgentSessionStore"/> class.
    /// </summary>
    public JsonAgentSessionStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = Path.GetFullPath(filePath);
    }

    /// <inheritdoc />
    public async Task SaveAsync(AgentSessionSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string temporaryFilePath = $"{_filePath}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (FileStream stream = new(
                temporaryFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync(stream, snapshot, JsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryFilePath, _filePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryFilePath))
            {
                File.Delete(temporaryFilePath);
            }
        }
    }

    /// <inheritdoc />
    public async Task<AgentSessionSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("Agent session snapshot was not found.", _filePath);
        }

        await using FileStream stream = File.OpenRead(_filePath);
        AgentSessionSnapshot snapshot = await JsonSerializer.DeserializeAsync<AgentSessionSnapshot>(
            stream,
            JsonOptions,
            cancellationToken)
            ?? throw new InvalidDataException("Agent session snapshot is empty or invalid.");
        if (string.IsNullOrWhiteSpace(snapshot.SessionId) || string.IsNullOrWhiteSpace(snapshot.WorkspacePath))
        {
            throw new InvalidDataException("Agent session snapshot does not contain session identity and workspace path.");
        }

        return snapshot;
    }
}
