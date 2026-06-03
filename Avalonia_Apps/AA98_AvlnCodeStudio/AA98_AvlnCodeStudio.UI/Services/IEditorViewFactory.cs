using Avalonia.Controls;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Creates Avalonia editor views for editor components.
/// </summary>
public interface IEditorViewFactory
{
    /// <summary>
    /// Creates an editor view for the specified view model.
    /// </summary>
    /// <param name="editorViewModel">The editor view model.</param>
    /// <returns>The created view.</returns>
    Control Create(ViewModels.EditorViewModel editorViewModel);
}