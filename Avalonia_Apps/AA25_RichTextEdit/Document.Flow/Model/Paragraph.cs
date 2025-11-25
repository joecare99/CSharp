namespace Document.Flow.Model;

/// <summary>
/// Represents a paragraph, a block-level content element.
/// </summary>
public class Paragraph : Block
{
    /// <summary>
    /// Gets the collection of inlines that make up the content of the paragraph.
    /// </summary>
    public InlineCollection Inlines { get; }

    public Paragraph()
    {
        Inlines = new InlineCollection(this);
    }

    public Paragraph(Inline inline) : this()
    {
        Inlines.Add(inline);
    }
}
