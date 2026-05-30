using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avln_Marble.Display.ViewModels;

namespace Avln_Marble.Display.Views;

/// <summary>
/// Hosts the browser-friendly marble board view.
/// </summary>
public partial class BrowserMainView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserMainView"/> class.
    /// </summary>
    /// <param name="viewModel">The root view model.</param>
    public BrowserMainView(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        AttachedToVisualTree += (_, _) => Focus();
    }

    private BoardViewModel Board => ((MainWindowViewModel)DataContext!).Board;

    private void BrowserMainView_OnKeyDown(object? sender, KeyEventArgs e)
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
