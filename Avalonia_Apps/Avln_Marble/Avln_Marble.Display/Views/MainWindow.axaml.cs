using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avln_Marble.Display.ViewModels;

namespace Avln_Marble.Display.Views;

/// <summary>
/// Hosts the interactive marble board prototype.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The root view model.</param>
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private BoardViewModel Board => ((MainWindowViewModel)DataContext!).Board;

    private void Window_OnKeyDown(object? sender, KeyEventArgs e)
    {
        bool shiftPressed = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        if (Board.HandleArrowKey(e.Key, shiftPressed))
        {
            e.Handled = true;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
