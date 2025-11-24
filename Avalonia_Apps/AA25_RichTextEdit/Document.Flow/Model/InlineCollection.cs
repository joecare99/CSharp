using System.Collections.Generic;

namespace Document.Flow.Model;

/// <summary>
/// Represents a collection of <see cref="Inline"/> elements.
/// </summary>
public class InlineCollection : List<Inline>
{
    private readonly Paragraph _parentParagraph;

    public InlineCollection(Paragraph parentParagraph)
    {
        _parentParagraph = parentParagraph;
    }

    public void Add(string text)
    {
        Add(new Run(text));
    }
}
