using Document.Base.Models.Interfaces;

namespace Document.Model.Models;

/// <summary>
/// Represents the source context used to begin a document import.
/// </summary>
/// <remarks>
/// Importers can use this record to pass source identity and known metadata into the document
/// population pipeline without coupling themselves to a specific PDF or renderer implementation.
/// </remarks>
public sealed record DocumentImportSourceModel : IDocumentImportSource
{
    /// <summary>
    /// Gets or sets the source path.
    /// </summary>
    public string? SourcePath { get; init; }

    /// <summary>
    /// Gets or sets the source name.
    /// </summary>
    public string? SourceName { get; init; }

    /// <summary>
    /// Gets or sets the media type.
    /// </summary>
    public string? MediaType { get; init; }

    /// <summary>
    /// Gets or sets the optional metadata payload.
    /// </summary>
    public IDocumentMetadata? Metadata { get; init; }
}
