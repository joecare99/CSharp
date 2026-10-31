using Avln_CommonDialogs.Base.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Adapts the shared Avalonia file picker to C# workspace files.
/// </summary>
public sealed class AvaloniaWorkspaceFileDialogService : IWorkspaceFileDialogService
{
    private static readonly FileTypeFilter WorkspaceFilter = new(
        "C# workspace files",
        new[] { "*.sln", "*.slnx", "*.csproj" });
    private static readonly FileTypeFilter AllFilesFilter = new("All files", new[] { "*" });

    private readonly IOpenFileDialog _openFileDialog;

    public AvaloniaWorkspaceFileDialogService(IOpenFileDialog openFileDialog)
    {
        _openFileDialog = openFileDialog;
    }

    /// <inheritdoc/>
    public async Task<string?> ShowOpenWorkspaceDialogAsync(
        string? initialDirectory,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _openFileDialog.Title = "Open C# workspace";
        _openFileDialog.InitialDirectory = initialDirectory;
        _openFileDialog.AllowMultiple = false;
        _openFileDialog.MutableFilters.Clear();
        _openFileDialog.MutableFilters.Add(WorkspaceFilter);
        _openFileDialog.MutableFilters.Add(AllFilesFilter);

        var files = await _openFileDialog.ShowAsync().ConfigureAwait(false);
        return files.Count > 0 ? files[0] : null;
    }
}
