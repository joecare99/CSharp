namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a content-bearing document element that can carry text and inline formatting.
/// </summary>
/// <remarks>
/// This interface is designed for text-centric document objects such as paragraphs, headings,
/// spans, and table-of-contents entries. It exposes a small set of text composition helpers that
/// allow callers to build formatted text without depending on a renderer.
/// </remarks>
public interface IDocContent : IDocElement
{
    /// <summary>
    /// Gets or sets the raw text content of the element.
    /// </summary>
    /// <remarks>
    /// Implementations may normalize whitespace or preserve it depending on the document type.
    /// The property is intended for the textual payload of the element, not for layout metadata.
    /// </remarks>
    string TextContent { get; set; }

    /// <summary>
    /// Appends plain text to the element.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <remarks>
    /// This method is typically used for incremental text building when a caller wants to preserve
    /// the current formatting context.
    /// </remarks>
    void AppendText(string text);

    /// <summary>
    /// Appends a line break to the content.
    /// </summary>
    /// <returns>The current content node for fluent chaining.</returns>
    IDocContent AddLineBreak();

    /// <summary>
    /// Appends a non-breaking space using the supplied font style context.
    /// </summary>
    /// <param name="docFontStyle">The font style to associate with the inserted space.</param>
    /// <returns>The current content node for fluent chaining.</returns>
    IDocContent AddNBSpace(IDocFontStyle docFontStyle);

    /// <summary>
    /// Appends a tab character using the supplied font style context.
    /// </summary>
    /// <param name="docFontStyle">The font style to associate with the inserted tab.</param>
    /// <returns>The current content node for fluent chaining.</returns>
    IDocContent AddTab(IDocFontStyle docFontStyle);

    /// <summary>
    /// Creates a span using the supplied font style.
    /// </summary>
    /// <param name="docFontStyle">The font style to apply.</param>
    /// <returns>The created span.</returns>
    IDocSpan AddSpan(IDocFontStyle docFontStyle);

    /// <summary>
    /// Creates a span with a loose style description.
    /// </summary>
    /// <param name="text">The span text.</param>
    /// <param name="docFontStyle">A style description payload.</param>
    /// <returns>The created span.</returns>
    /// <remarks>
    /// This overload exists for scenarios where style information is available in a non-typed
    /// form and must be passed through unchanged.
    /// </remarks>
    IDocSpan AddSpan(string text, IList<object> docFontStyle);

    /// <summary>
    /// Creates a span with a typed font style.
    /// </summary>
    /// <param name="text">The span text.</param>
    /// <param name="docFontStyle">The font style to apply.</param>
    /// <returns>The created span.</returns>
    IDocSpan AddSpan(string text, IDocFontStyle docFontStyle);

    /// <summary>
    /// Creates a span using an enum-based font style hint.
    /// </summary>
    /// <param name="text">The span text.</param>
    /// <param name="eFontStyle">The enum-based font style.</param>
    /// <returns>The created span.</returns>
    IDocSpan AddSpan(string text, EFontStyle eFontStyle);

    /// <summary>
    /// Creates a hyperlink span.
    /// </summary>
    /// <param name="Href">The link target.</param>
    /// <param name="docFontStyle">The font style to apply to the link.</param>
    /// <returns>The created span.</returns>
    IDocSpan AddLink(string Href, IDocFontStyle docFontStyle);

    /// <summary>
    /// Gets the current style object used by the element.
    /// </summary>
    /// <returns>The current style description.</returns>
    IDocStyleStyle GetStyle();

    /// <summary>
    /// Returns the textual content of the element.
    /// </summary>
    /// <param name="xRecursive">When <c>true</c>, include text from child nodes where applicable.</param>
    /// <returns>The text content.</returns>
    string GetTextContent(bool xRecursive = true);
}
