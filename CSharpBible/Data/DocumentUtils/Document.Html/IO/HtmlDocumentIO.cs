using System.Text;
using Document.Html.Model;
using Document.Html.Serialization;

namespace Document.Html.IO;

public static class HtmlDocumentIO
{
    public static async Task<HtmlSection> LoadAsync(string path, CancellationToken ct = default)
    {
        var html = await File.ReadAllTextAsync(path, Encoding.UTF8, ct);
        return await HtmlDocumentSerializer.FromHtmlStringAsync(html);
    }

    public static async Task SaveAsync(HtmlSection root, string path, CancellationToken ct = default)
    {
        var html = HtmlDocumentSerializer.ToHtmlString(root);
        await File.WriteAllTextAsync(path, html, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), ct);
    }
}
