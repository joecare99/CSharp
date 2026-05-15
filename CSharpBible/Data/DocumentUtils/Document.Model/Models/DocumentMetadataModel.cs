using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents document-level descriptive metadata.
/// </summary>
/// <remarks>
/// The metadata record is intentionally immutable so that source information can be carried
/// forward safely through analysis and rendering pipelines without accidental mutation.
/// </remarks>
public sealed record DocumentMetadataModel(
    /// <summary>
    /// The document title, if available.
    /// </summary>
    string? Title = null,
    /// <summary>
    /// The document author, if available.
    /// </summary>
    string? Author = null,
    /// <summary>
    /// The document subject, if available.
    /// </summary>
    string? Subject = null,
    /// <summary>
    /// Keywords associated with the document, if available.
    /// </summary>
    string? Keywords = null,
    /// <summary>
    /// The creating application or tool, if available.
    /// </summary>
    string? Creator = null,
    /// <summary>
    /// The producing application or tool, if available.
    /// </summary>
    string? Producer = null,
    /// <summary>
    /// The creation timestamp, if available.
    /// </summary>
    DateTimeOffset? Created = null,
    /// <summary>
    /// The last modification timestamp, if available.
    /// </summary>
    DateTimeOffset? Modified = null,
    /// <summary>
    /// The source file name, if available.
    /// </summary>
    string? FileName = null,
    /// <summary>
    /// The source path, if available.
    /// </summary>
    string? SourcePath = null) : IDocumentMetadata;
