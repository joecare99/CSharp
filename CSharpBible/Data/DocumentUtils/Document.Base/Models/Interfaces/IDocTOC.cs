namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a table-of-contents element.
/// </summary>
/// <remarks>
/// A TOC element can rebuild itself from a document section tree. This makes it suitable for
/// automatically generated navigation structures that mirror the current document content.
/// </remarks>
public interface IDocTOC : IDocContent
{
    /// <summary>
    /// Rebuilds the table of contents from the supplied document section tree.
    /// </summary>
    /// <param name="root">The root section to scan.</param>
    /// <remarks>
    /// Implementations are expected to traverse the section hierarchy and create a navigation
    /// structure that reflects headings and other TOC-relevant content.
    /// </remarks>
    void RebuildFrom(IDocSection root);
}
