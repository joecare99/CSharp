using AA98_AvlnCodeStudio.Editor.Components;
using Avalonia.Controls;

namespace AA98_AvlnCodeStudio.UI.Components;

/// <summary>
/// Represents the Avalonia-hosted editor component.
/// </summary>
public interface IAvaloniaEditorComponent : IEditorComponent
{
    /// <summary>
    /// Gets the editor view model.
    /// </summary>
    ViewModels.EditorViewModel EditorViewModel { get; }

    /// <summary>
    /// Gets the Avalonia view for the component.
    /// </summary>
    Control View { get; }
}