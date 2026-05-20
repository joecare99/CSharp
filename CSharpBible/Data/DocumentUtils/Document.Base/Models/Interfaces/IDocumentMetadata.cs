namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents renderer-neutral document metadata.
/// </summary>
/// <remarks>
/// The metadata contract is deliberately small and source-agnostic so that document importers can
/// expose common descriptive fields without committing to a specific file format or renderer.
/// Implementations may leave properties unset when the source does not provide reliable values.
/// </remarks>
public interface IDocumentMetadata
{
    /// <summary>
    /// Gets the document title.
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// Gets the document author.
    /// </summary>
    string? Author { get; }

    /// <summary>
    /// Gets the document subject.
    /// </summary>
    string? Subject { get; }

    /// <summary>
    /// Gets document keywords.
    /// </summary>
    string? Keywords { get; }

    /// <summary>
    /// Gets the creator application or tool.
    /// </summary>
    string? Creator { get; }

    /// <summary>
    /// Gets the producer application or tool.
    /// </summary>
    string? Producer { get; }

    /// <summary>
    /// Gets the creation timestamp, if available.
    /// </summary>
    DateTimeOffset? Created { get; }

    /// <summary>
    /// Gets the last modification timestamp, if available.
    /// </summary>
    DateTimeOffset? Modified { get; }

    /// <summary>
    /// Gets the source file name, if known.
    /// </summary>
    string? FileName { get; }

    /// <summary>
    /// Gets the source path, if known.
    /// </summary>
    string? SourcePath { get; }
}
