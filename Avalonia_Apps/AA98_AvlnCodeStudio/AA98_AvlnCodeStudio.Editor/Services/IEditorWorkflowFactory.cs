namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>
/// Creates editor workflow instances for composed editor components.
/// </summary>
public interface IEditorWorkflowFactory
{
    /// <summary>
    /// Creates a new editor workflow instance.
    /// </summary>
    /// <returns>The created workflow.</returns>
    IEditorWorkflow Create();
}