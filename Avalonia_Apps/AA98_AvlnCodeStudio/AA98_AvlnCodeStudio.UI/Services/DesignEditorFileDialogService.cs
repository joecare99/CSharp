using AA98_AvlnCodeStudio.Base.Services;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Provides a no-op dialog implementation for design-time preview support.
/// </summary>
public sealed class DesignEditorFileDialogService : IEditorFileDialogService
{
    /// <inheritdoc/>
    public Task<string?> ShowOpenFileDialogAsync(string? initialDirectory, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    /// <inheritdoc/>
    public Task<string?> ShowSaveFileDialogAsync(string? initialDirectory, string? initialFileName, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);
}
