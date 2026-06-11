using AA40_Wizzard.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA40_Wizzard.Views;

/// <summary>
/// Hosts the main shell content for both desktop and browser lifetimes.
/// </summary>
public partial class MainView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    public MainView()
        => AvaloniaXamlLoader.Load(this);

    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public MainView(MainWindowViewModel viewModel) : this()
        => DataContext = viewModel;
}
