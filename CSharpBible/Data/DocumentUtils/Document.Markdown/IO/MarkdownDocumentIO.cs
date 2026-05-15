using System.Text;
using Document.Markdown.Model;
using Document.Markdown.Serialization;

namespace Document.Markdown.IO;

public static class MarkdownDocumentIO
{
    public static async Task<MarkdownSection> LoadAsync(string path, CancellationToken ct = default)
    {
        string markdown = await File.ReadAllTextAsync(path, Encoding.UTF8, ct).ConfigureAwait(false);
        return await MarkdownDocumentSerializer.FromMarkdownStringAsync(markdown).ConfigureAwait(false);
    }

    public static async Task SaveAsync(MarkdownSection root, string path, CancellationToken ct = default)
    {
        string markdown = MarkdownDocumentSerializer.ToMarkdownString(root);
        await File.WriteAllTextAsync(path, markdown, ct).ConfigureAwait(false);
    }
}
