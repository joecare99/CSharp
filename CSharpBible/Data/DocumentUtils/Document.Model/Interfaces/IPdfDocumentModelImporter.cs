using Document.Base.Models.Interfaces;

namespace Document.Model.Interfaces;

/// <summary>
/// Defines the source-agnostic entry point for populating a canonical document model.
/// </summary>
/// <remarks>
/// The importer is responsible for reading a document source, extracting structure, text, images,
/// drawings, and analysis hints, and then mapping them into the shared document model through the
/// writer interface. The interface intentionally stays small so that adapters can remain
/// deterministic, easy to test, and reusable across document types.
/// 
/// <para>
/// PDF-specific implementations are expected to live in the separate <c>Document.Pdf</c> project
/// and can be composed into the application through dependency injection.
/// </para>
/// </remarks>
public interface IDocumentModelImporter
{
    /// <summary>
    /// Imports a document source into the canonical document model.
    /// </summary>
    /// <param name="source">The source context describing the document to import.</param>
    /// <param name="writer">The writer used to populate the document model.</param>
    /// <remarks>
    /// Implementations may read the source from the path or logical source information in the
    /// source model. The populated document should preserve source order and attach derived
    /// artifacts such as exported images, drawing blocks, and OCR-related hints.
    /// </remarks>
    void Import(IDocumentImportSource source, IDocumentModelWriter writer);
}
