namespace Document.Base.Models.Interfaces;

/// <summary>
/// Describes the origin of a document or document node.
/// </summary>
/// <remarks>
/// Source information is kept separate from the logical document structure so that the same
/// model can be used for files, streams, generated documents, and later derived artifacts.
/// </remarks>
public interface IDocumentSource
{
    /// <summary>
    /// Gets the optional file system path or external source path.
    /// </summary>
    /// <remarks>
    /// This value is useful when the document originated from a file. It should not be used
    /// as the only identity for the document because in-memory or virtual sources may not have one.
    /// </remarks>
    string? SourcePath { get; }

    /// <summary>
    /// Gets the friendly source name.
    /// </summary>
    /// <remarks>
    /// This may be a file name, display name, or a logical name assigned by the importer.
    /// It is intended for diagnostics and user-facing descriptions.
    /// </remarks>
    string? SourceName { get; }

    /// <summary>
    /// Gets the media type of the source when available.
    /// </summary>
    /// <remarks>
    /// Examples include <c>application/pdf</c> or <c>image/png</c>. The value is optional
    /// because some sources are only known structurally and not by MIME type.
    /// </remarks>
    string? MediaType { get; }
}
