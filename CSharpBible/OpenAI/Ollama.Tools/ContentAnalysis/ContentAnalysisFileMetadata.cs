using System;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents optional file metadata for a content analysis request.
/// </summary>
public sealed class ContentAnalysisFileMetadata
{
    /// <summary>
    /// Gets the file name when known.
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the file extension when known.
    /// </summary>
    public string Extension { get; init; } = string.Empty;

    /// <summary>
    /// Gets the file size in bytes when known.
    /// </summary>
    public long? SizeBytes { get; init; }

    /// <summary>
    /// Gets the last write timestamp in UTC when known.
    /// </summary>
    public DateTimeOffset? LastWriteTimeUtc { get; init; }
}
