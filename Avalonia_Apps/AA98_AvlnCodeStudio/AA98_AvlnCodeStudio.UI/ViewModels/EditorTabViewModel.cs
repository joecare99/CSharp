using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Represents one open editor document and its composed view.
/// </summary>
public sealed partial class EditorTabViewModel : ObservableObject
{
    public EditorTabViewModel(IAvaloniaEditorComponent component)
    {
        Component = component ?? throw new ArgumentNullException(nameof(component));
        Editor = component.EditorViewModel;
        Editor.PropertyChanged += OnEditorPropertyChanged;
    }

    public IAvaloniaEditorComponent Component { get; }

    public EditorViewModel Editor { get; }

    public Avalonia.Controls.Control View => Component.View;

    public string Title => $"{Editor.DocumentName ?? Editor.WindowTitle}{(Editor.IsDirty ? UiStrings.WindowTitleDirtySuffix : string.Empty)}";

    public string? FilePath => Editor.CurrentFilePath;

    private void OnEditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(EditorViewModel.DocumentName)
            or nameof(EditorViewModel.CurrentFilePath)
            or nameof(EditorViewModel.IsDirty))
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(FilePath));
        }
    }
}
