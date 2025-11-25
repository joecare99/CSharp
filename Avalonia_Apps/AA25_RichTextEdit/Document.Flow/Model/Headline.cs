namespace Document.Flow.Model;

/// <summary>
/// Represents a headline, a block-level content element.
/// </summary>
public class Headline : Block
{
    /// <summary>
    /// Gets or sets the level of the headline (e.g., 1 for H1, 2 for H2).
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets the collection of inlines that make up the content of the headline.
    /// </summary>
    public InlineCollection Inlines { get; }

    public Headline(int level = 1)
    {
        Level = level;
        Inlines = new InlineCollection(new Paragraph()); // Simplified: re-use paragraph logic for inlines
    }

    public Headline(int level, Inline inline) : this(level)
    {
        Inlines.Add(inline);
    }
}
