using AA98_AvlnCodeStudio.UI.Controls;
using Avalonia.Controls;
using Avalonia.Data;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Creates Avalonia editor views and binds them to editor view models.
/// </summary>
public sealed class AvaloniaEditorViewFactory : IEditorViewFactory
{
    /// <inheritdoc/>
    public Control Create(ViewModels.EditorViewModel editorViewModel)
    {
        var editorView = new EditorTextArea
        {
            DataContext = editorViewModel,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
        };

        editorView.Bind(EditorTextArea.TextProperty, new Binding(nameof(ViewModels.EditorViewModel.Text))
        {
            Mode = BindingMode.TwoWay,
        });

        return editorView;
    }
}