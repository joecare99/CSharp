using AA98_AvlnCodeStudio.Editor.Services;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Creates editor view models around a workflow core.
/// </summary>
public sealed class EditorViewModelFactory : IEditorViewModelFactory
{
    /// <inheritdoc/>
    public EditorViewModel Create(IEditorWorkflow workflow)
    {
        return new EditorViewModel(workflow);
    }
}