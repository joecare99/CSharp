using AA40_Wizzard.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA40_Wizzard.Views;

/// <summary>
/// Hosts the desktop shell for the wizard sample.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
        => AvaloniaXamlLoader.Load(this);

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public MainWindow(MainWindowViewModel viewModel) : this()
        => DataContext = viewModel;
}
