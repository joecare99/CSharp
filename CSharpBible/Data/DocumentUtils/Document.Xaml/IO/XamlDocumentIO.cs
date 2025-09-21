using System.Text;
using Document.Xaml.Model;
using Document.Xaml.Serialization;

namespace Document.Xaml.IO;

public static class XamlDocumentIO
{
    public static async Task SaveAsync(XamlSection root, string path, CancellationToken ct = default)
    {
        var xaml = XamlDocumentSerializer.ToXamlString(root);
        await File.WriteAllTextAsync(path, xaml, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), ct);
    }

    // Platzhalter für künftiges Laden
    public static Task<XamlSection> LoadAsync(string path, CancellationToken ct = default)
        => Task.FromResult(new XamlSection());
}