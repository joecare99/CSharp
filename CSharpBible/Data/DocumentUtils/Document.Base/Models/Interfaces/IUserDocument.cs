namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a legacy user-facing document abstraction.
/// </summary>
/// <remarks>
/// This interface exposes creation, traversal, loading, and saving operations for the older
/// document tree contract. It remains useful as a compatibility layer while the richer document
/// model evolves in a separate project.
/// </remarks>
public interface IUserDocument
{
    /// <summary>
    /// Adds a paragraph using the specified style name.
    /// </summary>
    IDocParagraph AddParagraph(string cStylename);

    /// <summary>
    /// Adds a heading at the given level.
    /// </summary>
    IDocHeadline AddHeadline(int nLevel, string? Id = null);

    /// <summary>
    /// Adds a table-of-contents node.
    /// </summary>
    IDocTOC AddTOC(string cName, int nLevel);

    /// <summary>
    /// Gets the root element of the legacy document tree.
    /// </summary>
    IDocElement Root { get; }

    /// <summary>
    /// Enumerates the document tree.
    /// </summary>
    IEnumerable<IDocElement> Enumerate();

    /// <summary>
    /// Gets a value indicating whether the document has unsaved changes.
    /// </summary>
    bool IsModified { get; }

    /// <summary>
    /// Saves the document to a file path.
    /// </summary>
    bool SaveTo(string cOutputPath);

    /// <summary>
    /// Saves the document to a stream.
    /// </summary>
    bool SaveTo(Stream sOutputStream, object? options = null);

    /// <summary>
    /// Loads the document from a file path.
    /// </summary>
    bool LoadFrom(string cInputPath);

    /// <summary>
    /// Loads the document from a stream.
    /// </summary>
    bool LoadFrom(Stream sInputStream, object? options = null);
}
