using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public sealed class MarkdownStyle : IDocStyleStyle
{
    public MarkdownStyle(string? name)
    {
        Name = name;
    }

    public string? Name { get; }

    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
