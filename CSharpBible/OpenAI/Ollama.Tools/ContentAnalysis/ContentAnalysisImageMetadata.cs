namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents optional image-specific metadata for a content analysis request.
/// </summary>
public sealed class ContentAnalysisImageMetadata
{
    /// <summary>
    /// Gets the detected image width in pixels when known.
    /// </summary>
    public int? PixelWidth { get; init; }

    /// <summary>
    /// Gets the detected image height in pixels when known.
    /// </summary>
    public int? PixelHeight { get; init; }

    /// <summary>
    /// Gets the detected bits per pixel when known.
    /// </summary>
    public int? BitsPerPixel { get; init; }

    /// <summary>
    /// Gets the detected image format label when known.
    /// </summary>
    public string Format { get; init; } = string.Empty;
}
