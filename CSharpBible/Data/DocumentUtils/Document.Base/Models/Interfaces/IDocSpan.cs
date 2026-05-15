namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents an inline span of text with formatting information.
/// </summary>
/// <remarks>
/// Spans are the inline building blocks used by paragraphs and headings. They are responsible
/// for preserving formatting, hyperlinks, and style references while remaining lightweight.
/// </remarks>
public interface IDocSpan : IDocContent
{
    /// <summary>
    /// Applies a style from an untyped style object.
    /// </summary>
    /// <param name="fs">The style payload.</param>
    void SetStyle(object fs);

    /// <summary>
    /// Applies a typed font style.
    /// </summary>
    /// <param name="fs">The font style.</param>
    void SetStyle(IDocFontStyle fs);

    /// <summary>
    /// Applies a style from a document context and untyped font descriptor.
    /// </summary>
    /// <param name="doc">The owning document.</param>
    /// <param name="aFont">The font payload.</param>
    void SetStyle(IUserDocument doc, object aFont);

    /// <summary>
    /// Applies a style from a document context and typed font descriptor.
    /// </summary>
    /// <param name="doc">The owning document.</param>
    /// <param name="aFont">The font style.</param>
    void SetStyle(IUserDocument doc, IDocFontStyle aFont);

    /// <summary>
    /// Applies a named style.
    /// </summary>
    /// <param name="aStyleName">The name of the style to apply.</param>
    void SetStyle(string aStyleName);

    /// <summary>
    /// Gets or sets the span identifier.
    /// </summary>
    /// <remarks>
    /// The identifier can be used for anchors, bookmarks, or linking to a specific inline region.
    /// </remarks>
    string? Id { get; set; }
}
