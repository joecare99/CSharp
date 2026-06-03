using AA98_AvlnCodeStudio.Editor.Services;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Creates editor view model instances for a workflow.
/// </summary>
public interface IEditorViewModelFactory
{
    /// <summary>
    /// Creates an editor view model for the specified workflow.
    /// </summary>
    /// <param name="workflow">The editor workflow.</param>
    /// <returns>The created editor view model.</returns>
    EditorViewModel Create(IEditorWorkflow workflow);
}