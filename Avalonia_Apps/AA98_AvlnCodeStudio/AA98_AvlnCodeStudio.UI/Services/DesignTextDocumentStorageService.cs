using AA98_AvlnCodeStudio.Base.Services;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Provides a no-op storage implementation for design-time preview support.
/// </summary>
public sealed class DesignTextDocumentStorageService : ITextDocumentStorageService
{
    /// <inheritdoc/>
    public Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default)
        => Task.FromResult(string.Empty);

    /// <inheritdoc/>
    public Task SaveAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
