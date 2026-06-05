using AA98_AvlnCodeStudio.Editor.Components;

namespace AA98_AvlnCodeStudio.UI.Components;

/// <summary>
/// Creates Avalonia-hosted editor component instances.
/// </summary>
public interface IAvaloniaEditorComponentFactory : IEditorComponentFactory
{
    /// <summary>
    /// Creates the editor component.
    /// </summary>
    /// <returns>The created editor component.</returns>
    IAvaloniaEditorComponent Create();
}