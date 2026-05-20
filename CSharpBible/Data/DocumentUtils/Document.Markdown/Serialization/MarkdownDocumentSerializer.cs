using Document.Markdown.Model;
using Document.Markdown.Rendering;

namespace Document.Markdown.Serialization;

public static class MarkdownDocumentSerializer
{
    public static Task<MarkdownSection> FromMarkdownStringAsync(string markdown)
    {
        MarkdownSection root = new();
        if (!string.IsNullOrWhiteSpace(markdown))
        {
            MarkdownParagraph paragraph = new(null);
            paragraph.TextContent = markdown;
            root.AddChild(paragraph);
        }

        return Task.FromResult(root);
    }

    public static string ToMarkdownString(MarkdownSection root)
        => MarkdownRenderer.Render(root);
}
