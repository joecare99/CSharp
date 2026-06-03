using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;

namespace AA98_AvlnCodeStudio.UI.Components;

/// <summary>
/// Creates the first explicit Avalonia editor component.
/// </summary>
public sealed class AvaloniaEditorComponentFactory : IAvaloniaEditorComponentFactory, IEditorComponentFactory
{
    private readonly Services.IEditorViewFactory _editorViewFactory;
    private readonly ViewModels.IEditorViewModelFactory _editorViewModelFactory;
    private readonly IEditorWorkflowFactory _workflowFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaEditorComponentFactory"/> class.
    /// </summary>
    /// <param name="workflowFactory">The workflow factory.</param>
    /// <param name="editorViewModelFactory">The editor view model factory.</param>
    /// <param name="editorViewFactory">The editor view factory.</param>
    public AvaloniaEditorComponentFactory(
        IEditorWorkflowFactory workflowFactory,
        ViewModels.IEditorViewModelFactory editorViewModelFactory,
        Services.IEditorViewFactory editorViewFactory)
    {
        _workflowFactory = workflowFactory;
        _editorViewModelFactory = editorViewModelFactory;
        _editorViewFactory = editorViewFactory;
    }

    /// <inheritdoc/>
    public IAvaloniaEditorComponent Create()
    {
        var workflow = _workflowFactory.Create();
        var editorViewModel = _editorViewModelFactory.Create(workflow);
        var view = _editorViewFactory.Create(editorViewModel);
        return new AvaloniaEditorComponent(workflow, editorViewModel, view);
    }

    /// <inheritdoc/>
    IEditorComponent IEditorComponentFactory.Create()
    {
        return Create();
    }
}