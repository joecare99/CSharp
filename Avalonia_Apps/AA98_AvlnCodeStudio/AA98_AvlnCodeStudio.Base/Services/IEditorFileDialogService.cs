using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Services;

/// <summary>
/// Defines UI-agnostic file picker operations for the editor workflow.
/// </summary>
public interface IEditorFileDialogService
{
    /// <summary>
    /// Opens a file picker for selecting an existing text file.
    /// </summary>
    /// <param name="initialDirectory">The optional initial directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The selected file path, or <see langword="null"/> when the dialog was canceled.</returns>
    Task<string?> ShowOpenFileDialogAsync(string? initialDirectory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a file picker for selecting the output path of a text file.
    /// </summary>
    /// <param name="initialDirectory">The optional initial directory.</param>
    /// <param name="initialFileName">The optional initial file name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The selected file path, or <see langword="null"/> when the dialog was canceled.</returns>
    Task<string?> ShowSaveFileDialogAsync(string? initialDirectory, string? initialFileName, CancellationToken cancellationToken = default);
}
