using AA98_AvlnCodeStudio.Model.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>
/// Defines the UI-agnostic workflow for a single text editor component.
/// </summary>
public interface IEditorWorkflow
{
    /// <summary>
    /// Gets the underlying document state.
    /// </summary>
    IFileEditorDocument Document { get; }

    /// <summary>
    /// Updates the current editor text.
    /// </summary>
    /// <param name="text">The new text value.</param>
    void UpdateText(string? text);

    /// <summary>
    /// Creates a new empty document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The workflow result.</returns>
    Task<EditorOperationResult> NewDocumentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens an existing text document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The workflow result.</returns>
    Task<EditorOperationResult> OpenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the current document.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The workflow result.</returns>
    Task<EditorOperationResult> SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the current document under a new file path.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The workflow result.</returns>
    Task<EditorOperationResult> SaveAsAsync(CancellationToken cancellationToken = default);
}