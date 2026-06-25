using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using Terminal.Avalonia.Controls;
using Terminal.Avalonia.ViewModels;
using Terminal.Core;

namespace Terminal.Avalonia.Host;

public sealed class MainWindow : Window
{
    private const string DefaultTitle = "Terminal.Avalonia Host";

    public MainWindow()
    {
        var terminalControl = new TerminalControl
        {
            SessionOptions = TerminalShellOptions.CreateDefault(),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        Title = DefaultTitle;
        Width = 960;
        Height = 640;
        MinWidth = 640;
        MinHeight = 400;
        Padding = new Thickness(12);
        Content = terminalControl;

        if (terminalControl.ViewModel is TerminalControlViewModel viewModel)
        {
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(TerminalControlViewModel.Title))
                {
                    Dispatcher.UIThread.Post(() => Title = string.IsNullOrWhiteSpace(viewModel.Title) ? DefaultTitle : viewModel.Title);
                }
            };
        }

        Opened += (_, _) => terminalControl.Focus();
    }
}
