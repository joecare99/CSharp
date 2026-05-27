using AA98_AvlnCodeStudio.Base.Services;
using Avln_CommonDialogs.Base.Interfaces;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Adapts shared file dialogs to the editor-specific workflow.
/// </summary>
public sealed class AvaloniaEditorFileDialogService : IEditorFileDialogService
{
    private static readonly FileTypeFilter TextDocumentsFilter = new("Text documents", new[] { "*.txt", "*.md", "*.cs", "*.axaml", "*.json", "*.xml" });
    private static readonly FileTypeFilter AllFilesFilter = new("All files", new[] { "*" });

    private readonly IOpenFileDialog _openFileDialog;
    private readonly ISaveFileDialog _saveFileDialog;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaEditorFileDialogService"/> class.
    /// </summary>
    /// <param name="openFileDialog">The open file dialog implementation.</param>
    /// <param name="saveFileDialog">The save file dialog implementation.</param>
    public AvaloniaEditorFileDialogService(IOpenFileDialog openFileDialog, ISaveFileDialog saveFileDialog)
    {
        _openFileDialog = openFileDialog;
        _saveFileDialog = saveFileDialog;
    }

    /// <inheritdoc/>
    public async Task<string?> ShowOpenFileDialogAsync(string? initialDirectory, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _openFileDialog.Title = "Open text file";
        _openFileDialog.InitialDirectory = initialDirectory;
        _openFileDialog.AllowMultiple = false;
        _openFileDialog.MutableFilters.Clear();
        _openFileDialog.MutableFilters.Add(TextDocumentsFilter);
        _openFileDialog.MutableFilters.Add(AllFilesFilter);

        var files = await _openFileDialog.ShowAsync().ConfigureAwait(false);
        return files.Count > 0 ? files[0] : null;
    }

    /// <inheritdoc/>
    public async Task<string?> ShowSaveFileDialogAsync(string? initialDirectory, string? initialFileName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _saveFileDialog.Title = "Save text file";
        _saveFileDialog.InitialDirectory = initialDirectory;
        _saveFileDialog.InitialFileName = initialFileName;
        _saveFileDialog.DefaultExtension = "txt";
        _saveFileDialog.OverwritePrompt = true;
        _saveFileDialog.MutableFilters.Clear();
        _saveFileDialog.MutableFilters.Add(TextDocumentsFilter);
        _saveFileDialog.MutableFilters.Add(AllFilesFilter);

        return await _saveFileDialog.ShowAsync().ConfigureAwait(false);
    }
}
