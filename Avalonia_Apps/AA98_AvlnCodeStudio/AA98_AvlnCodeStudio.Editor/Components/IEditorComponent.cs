using AA98_AvlnCodeStudio.Editor.Services;

namespace AA98_AvlnCodeStudio.Editor.Components;

/// <summary>
/// Represents a composed editor component with a UI-agnostic workflow core.
/// </summary>
public interface IEditorComponent
{
    /// <summary>
    /// Gets the editor workflow core.
    /// </summary>
    IEditorWorkflow Workflow { get; }
}