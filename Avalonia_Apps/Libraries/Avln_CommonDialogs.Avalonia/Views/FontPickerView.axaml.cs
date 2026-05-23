using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avln_CommonDialogs.Avalonia.ViewModels;

namespace Avln_CommonDialogs.Avalonia.Views;

/// <summary>
/// Represents the reusable Avalonia font picker view.
/// </summary>
public partial class FontPickerView : UserControl
{
    /// <summary>
    /// Gets the OK button.
    /// </summary>
    public Button OkButton { get; private set; } = null!;

    /// <summary>
    /// Gets the Cancel button.
    /// </summary>
    public Button CancelButton { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPickerView"/> class.
    /// </summary>
    public FontPickerView()
    {
        AvaloniaXamlLoader.Load(this);
        OkButton = this.FindControl<Button>("btnOk")
            ?? throw new InvalidOperationException("The FontPickerView is missing its OK button.");
        CancelButton = this.FindControl<Button>("btnCancel")
            ?? throw new InvalidOperationException("The FontPickerView is missing its Cancel button.");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPickerView"/> class.
    /// </summary>
    /// <param name="viewModel">The font picker view model.</param>
    public FontPickerView(FontPickerViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}
