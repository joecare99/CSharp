using AA98_AvlnCodeStudio.Editor.Services;
using Avalonia.Controls;

namespace AA98_AvlnCodeStudio.UI.Components;

/// <summary>
/// Stores the composed Avalonia editor component parts.
/// </summary>
public sealed class AvaloniaEditorComponent : IAvaloniaEditorComponent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaEditorComponent"/> class.
    /// </summary>
    /// <param name="workflow">The editor workflow.</param>
    /// <param name="editorViewModel">The editor view model.</param>
    /// <param name="view">The editor view.</param>
    public AvaloniaEditorComponent(IEditorWorkflow workflow, ViewModels.EditorViewModel editorViewModel, Control view)
    {
        Workflow = workflow;
        EditorViewModel = editorViewModel;
        View = view;
    }

    /// <inheritdoc/>
    public IEditorWorkflow Workflow { get; }

    /// <inheritdoc/>
    public ViewModels.EditorViewModel EditorViewModel { get; }

    /// <inheritdoc/>
    public Control View { get; }
}