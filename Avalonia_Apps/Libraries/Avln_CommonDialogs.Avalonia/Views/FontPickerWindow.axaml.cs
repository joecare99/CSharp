using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Avln_CommonDialogs.Avalonia.ViewModels;

namespace Avln_CommonDialogs.Avalonia.Views;

/// <summary>
/// Represents the Avalonia font picker window.
/// </summary>
public partial class FontPickerWindow : Window
{
    /// <summary>
    /// Gets the embedded font picker view.
    /// </summary>
    public FontPickerView EmbeddedPickerView
        => Content as FontPickerView
            ?? this.GetVisualDescendants().OfType<FontPickerView>().FirstOrDefault()
            ?? throw new InvalidOperationException("The FontPickerWindow is missing its embedded FontPickerView.");

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPickerWindow"/> class.
    /// </summary>
    public FontPickerWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPickerWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The dialog view model.</param>
    public FontPickerWindow(FontPickerViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}
