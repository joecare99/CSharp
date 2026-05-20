namespace Document.Base.Models.Interfaces;

/// <summary>
/// Describes the source context used to start a document import.
/// </summary>
/// <remarks>
/// This interface carries the minimal information an importer needs before it can start reading
/// and interpreting a document. It is intentionally generic so that different source types can be
/// supported without changing the importer contract.
/// </remarks>
public interface IDocumentImportSource : IDocumentSource
{
    /// <summary>
    /// Gets the optional metadata that is already known before import begins.
    /// </summary>
    /// <remarks>
    /// This can be used to pass through metadata that was collected by a file system probe,
    /// archive reader, or host application before the actual import starts. The importer should
    /// treat the value as source metadata, not as a final rendered representation.
    /// </remarks>
    IDocumentMetadata? Metadata { get; }
}
