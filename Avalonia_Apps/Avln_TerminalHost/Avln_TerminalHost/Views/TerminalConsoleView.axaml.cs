using Avalonia.Controls;
using Avln_TerminalHost.ViewModels;

namespace Avln_TerminalHost.Views;

/// <summary>
/// Hosts the reusable Avalonia console control inside the terminal window.
/// </summary>
public partial class TerminalConsoleView : UserControl
{
    private readonly ContentControl? _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalConsoleView"/> class.
    /// </summary>
    public TerminalConsoleView()
    {
        InitializeComponent();
        _host = this.FindControl<ContentControl>("PART_Host");
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (_host is null)
        {
            return;
        }

        _host.Content = (DataContext as MainWindowViewModel)?.Console.Control;
    }
}
