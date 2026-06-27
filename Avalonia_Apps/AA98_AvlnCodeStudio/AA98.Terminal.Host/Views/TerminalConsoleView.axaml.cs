using AA98.Terminal.Host.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avln_TestConsole.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Views;

/// <summary>
/// Hosts the reusable Avalonia console control inside the terminal window.
/// </summary>
public partial class TerminalConsoleView : UserControl
{
    private static readonly TimeSpan ResizeDebounceDelay = TimeSpan.FromMilliseconds(100);
    private readonly ContentControl? _host;
    private CancellationTokenSource? _resizeCancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalConsoleView"/> class.
    /// </summary>
    public TerminalConsoleView()
    {
        InitializeComponent();
        _host = this.FindControl<ContentControl>("PART_Host");
        DataContextChanged += OnDataContextChanged;
        SizeChanged += OnSizeChanged;
        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_host is null)
        {
            return;
        }

        if (_host.Content is AvaloniaConsoleControl previousControl)
        {
            previousControl.TextInput -= OnConsoleTextInput;
            previousControl.KeyDown -= OnConsoleKeyDown;
        }

        var consoleControl = (DataContext as MainWindowViewModel)?.Console.Control;
        if (consoleControl is not null)
        {
            consoleControl.TextInput += OnConsoleTextInput;
            consoleControl.KeyDown += OnConsoleKeyDown;
        }

        _host.Content = consoleControl;
        ScheduleConsoleResize();
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
        => ScheduleConsoleResize();

    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        => ScheduleConsoleResize();

    private void OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        => CancelPendingResize();

    private void ScheduleConsoleResize()
    {
        CancelPendingResize();
        _resizeCancellationTokenSource = new CancellationTokenSource();
        _ = ApplyConsoleResizeAsync(_resizeCancellationTokenSource);
    }

    private async Task ApplyConsoleResizeAsync(CancellationTokenSource resizeCancellationTokenSource)
    {
        try
        {
            await Task.Delay(ResizeDebounceDelay, resizeCancellationTokenSource.Token);
            await Dispatcher.UIThread.InvokeAsync(() => ApplyConsoleResize(resizeCancellationTokenSource));
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            resizeCancellationTokenSource.Dispose();
            if (ReferenceEquals(_resizeCancellationTokenSource, resizeCancellationTokenSource))
            {
                _resizeCancellationTokenSource = null;
            }
        }
    }

    private void ApplyConsoleResize(CancellationTokenSource resizeCancellationTokenSource)
    {
        if (!ReferenceEquals(_resizeCancellationTokenSource, resizeCancellationTokenSource) || _host?.Content is not AvaloniaConsoleControl consoleControl)
        {
            return;
        }

        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var width = GetFirstPositive(_host.Bounds.Width, Bounds.Width, _host.Width, Width);
        var height = GetFirstPositive(_host.Bounds.Height, Bounds.Height, _host.Height, Height);
        if (width <= 0d || height <= 0d)
        {
            return;
        }

        var cellSize = consoleControl.CharacterCellSize;
        var columnCount = Math.Max(1, (int)Math.Floor(width / cellSize.Width));
        var rowCount = Math.Max(1, (int)Math.Floor(height / cellSize.Height));
        if (viewModel.Console.WindowWidth == columnCount && viewModel.Console.WindowHeight == rowCount)
        {
            return;
        }

        viewModel.Console.WindowWidth = columnCount;
        viewModel.Console.WindowHeight = rowCount;
    }

    private static double GetFirstPositive(params double[] values)
    {
        foreach (var value in values)
        {
            if (!double.IsNaN(value) && value > 0d)
            {
                return value;
            }
        }

        return 0d;
    }

    private void CancelPendingResize()
    {
        var resizeCancellationTokenSource = _resizeCancellationTokenSource;
        _resizeCancellationTokenSource = null;
        resizeCancellationTokenSource?.Cancel();
    }

    private async void OnConsoleTextInput(object? sender, TextInputEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel || string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        await viewModel.HandleConsoleTextInputAsync(e.Text);
        e.Handled = true;
    }

    private async void OnConsoleKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (!TryMapConsoleKey(e.Key, out var consoleKey))
        {
            return;
        }

        await viewModel.HandleConsoleSpecialKeyAsync(consoleKey);
        e.Handled = true;
    }

    private static bool TryMapConsoleKey(Key key, out ConsoleKey consoleKey)
    {
        switch (key)
        {
            case Key.Enter:
                consoleKey = ConsoleKey.Enter;
                return true;
            case Key.Back:
                consoleKey = ConsoleKey.Backspace;
                return true;
            case Key.Tab:
                consoleKey = ConsoleKey.Tab;
                return true;
            default:
                consoleKey = default;
                return false;
        }
    }
}