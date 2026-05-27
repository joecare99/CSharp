using AA98_AvlnCodeStudio.Base.Services;
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
    public Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return File.ReadAllTextAsync(filePath, cancellationToken);
    }

    /// <inheritdoc/>
    public Task SaveAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
    {
        return File.WriteAllTextAsync(filePath, content, Encoding.UTF8, cancellationToken);
    }
}
