namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a paragraph-level content element.
/// </summary>
/// <remarks>
/// Paragraphs are the primary building blocks for body text. They can contain spans, links, and
/// bookmarks while still behaving as a single logical text unit.
/// </remarks>
public interface IDocParagraph : IDocContent
{
    /// <summary>
    /// Gets the paragraph-level attributes applied by document providers.
    /// </summary>
    new IList<IDocAttributes> DocAttributes { get; }

    /// <summary>
    /// Adds a bookmark span to the paragraph.
    /// </summary>
    /// <param name="Id">The bookmark identifier.</param>
    /// <param name="docFontStyle">The font style to apply to the bookmark span.</param>
    /// <returns>The created bookmark span.</returns>
    /// <remarks>
    /// Bookmarks are typically used to create internal navigation targets or to mark important
    /// locations for later reference.
    /// </remarks>
    IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle);
}
