using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using Ollama.Protocol.Models;

namespace Ollama.Protocol.Parsing;

/// <summary>
/// Reads newline-delimited JSON chunks from an Ollama generate stream.
/// </summary>
public sealed class OllamaGenerateStreamReader
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaGenerateStreamReader"/> class.
    /// </summary>
    /// <param name="jsonSerializerOptions">The serializer options used to parse stream chunks.</param>
    public OllamaGenerateStreamReader(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    /// <summary>
    /// Reads streamed chunks from the provided response stream.
    /// </summary>
    /// <param name="stream">The NDJSON response stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of parsed response chunks.</returns>
    public async IAsyncEnumerable<OllamaGenerateResponseChunk> ReadChunksAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string? line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            OllamaGenerateResponseChunk? chunk = JsonSerializer.Deserialize<OllamaGenerateResponseChunk>(line, _jsonSerializerOptions);
            if (chunk is not null)
            {
                yield return chunk;
            }
        }
    }
}
