namespace AA98_AvlnCodeStudio.Editor.Components;

/// <summary>
/// Creates composed editor component instances.
/// </summary>
public interface IEditorComponentFactory
{
    /// <summary>
    /// Creates a new editor component instance.
    /// </summary>
    /// <returns>The composed editor component.</returns>
    IEditorComponent Create();
}