using AA98_AvlnCodeStudio.Base.OS.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Provides file system persistence for plain text documents.
/// </summary>
public sealed class FileSystemTextDocumentStorageService : ITextDocumentStorageService
{
    /// <inheritdoc/>
    public async Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        byte[] bytes = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
        return TextDocumentEncoding.Decode(bytes).Text;
    }

    /// <inheritdoc/>
    public async Task SaveAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
    {
        Encoding encoding = new UTF8Encoding(false, true);
        if (File.Exists(filePath))
        {
            byte[] existing = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
            encoding = TextDocumentEncoding.Decode(existing).Encoding;
        }

        byte[] bytes = TextDocumentEncoding.Encode(content, encoding);
        await File.WriteAllBytesAsync(filePath, bytes, cancellationToken).ConfigureAwait(false);
    }
}
