using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Provides a no-op workspace picker for design-time preview support.
/// </summary>
public sealed class DesignWorkspaceFileDialogService : IWorkspaceFileDialogService
{
    public Task<string?> ShowOpenWorkspaceDialogAsync(
        string? initialDirectory,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(null);
    }

    public Task<string?> ShowSaveCSharpProjectDialogAsync(
        string? initialDirectory,
        string? initialFileName,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(null);
    }
}
