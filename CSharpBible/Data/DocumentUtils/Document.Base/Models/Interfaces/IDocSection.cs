namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a section container in the legacy document tree contract.
/// </summary>
/// <remarks>
/// A section groups other document elements such as paragraphs, headings, and table-of-contents
/// nodes. It is a structural container rather than a content leaf.
/// </remarks>
public interface IDocSection : IDocElement
{
    /// <summary>
    /// Adds a paragraph to the section.
    /// </summary>
    /// <param name="ATextStyleName">The name of the paragraph style to apply.</param>
    /// <returns>The created paragraph.</returns>
    IDocParagraph AddParagraph(string ATextStyleName);

    /// <summary>
    /// Adds a heading to the section.
    /// </summary>
    /// <param name="aLevel">The heading level.</param>
    /// <param name="Id">The heading identifier.</param>
    /// <returns>The created heading.</returns>
    IDocHeadline AddHeadline(int aLevel, string Id);

    /// <summary>
    /// Adds a table-of-contents node to the section.
    /// </summary>
    /// <param name="aName">The TOC name or title.</param>
    /// <param name="aLevel">The TOC nesting level.</param>
    /// <returns>The created TOC node.</returns>
    IDocTOC AddTOC(string aName, int aLevel);
}
