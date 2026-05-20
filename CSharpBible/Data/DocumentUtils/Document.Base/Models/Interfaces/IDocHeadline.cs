namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a heading or title-like document element.
/// </summary>
/// <remarks>
/// Headings are content elements that have a hierarchical level and can be used to build table
/// of contents structures, anchors, and navigation links.
/// </remarks>
public interface IDocHeadline : IDocContent
{
    /// <summary>
    /// Gets the heading level.
    /// </summary>
    /// <remarks>
    /// Lower values usually represent higher-level headings, but the exact mapping is defined by
    /// the document implementation or the renderer.
    /// </remarks>
    int Level { get; }

    /// <summary>
    /// Gets the unique identifier of the heading.
    /// </summary>
    /// <remarks>
    /// This identifier is commonly used to create anchors, hyperlinks, and table-of-contents
    /// entries. It should be stable and unique within the scope of the document.
    /// </remarks>
    string Id { get; }
}
