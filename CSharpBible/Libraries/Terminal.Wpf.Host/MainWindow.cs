using System.Windows;
using Terminal.Core;
using Terminal.Wpf.Controls;
using Terminal.Wpf.ViewModels;

namespace Terminal.Wpf.Host;

public sealed class MainWindow : Window
{
    private const string DefaultTitle = "Terminal.Wpf Host";

    public MainWindow()
    {
        var terminalControl = new TerminalControl
        {
            SessionOptions = TerminalShellOptions.CreateDefault(),
            Margin = new Thickness(12)
        };

        Title = DefaultTitle;
        Width = 960;
        Height = 640;
        MinWidth = 640;
        MinHeight = 400;
        Content = terminalControl;

        if (terminalControl.ViewModel is TerminalControlViewModel viewModel)
        {
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(TerminalControlViewModel.Title))
                {
                    Dispatcher.InvokeAsync(() => Title = string.IsNullOrWhiteSpace(viewModel.Title) ? DefaultTitle : viewModel.Title);
                }
            };
        }

        Loaded += (_, _) => terminalControl.Focus();
    }
}
