using System;
using System.Collections.Generic;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents one content analysis request.
/// </summary>
public sealed class ContentAnalysisRequest
{
    /// <summary>
    /// Gets the declared content kind.
    /// </summary>
    public OllamaContentKind ContentKind { get; init; }

    /// <summary>
    /// Gets how the content is supplied.
    /// </summary>
    public OllamaContentSourceKind SourceKind { get; init; }

    /// <summary>
    /// Gets the optional display name of the content.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the media type of the content.
    /// </summary>
    public string MediaType { get; init; } = "text/plain";

    /// <summary>
    /// Gets the declared programming or content language.
    /// </summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Gets the inline content payload.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the referenced file path.
    /// </summary>
    public string FilePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional file metadata.
    /// </summary>
    public ContentAnalysisFileMetadata FileMetadata { get; init; } = new();

    /// <summary>
    /// Gets the optional image-specific metadata.
    /// </summary>
    public ContentAnalysisImageMetadata ImageMetadata { get; init; } = new();

    /// <summary>
    /// Gets the requested evaluation criteria.
    /// </summary>
    public IReadOnlyList<ContentAnalysisCriterion> Criteria { get; init; } = Array.Empty<ContentAnalysisCriterion>();
}
