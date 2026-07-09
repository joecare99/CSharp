using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Terminal.Core;
using Terminal.Wpf.ViewModels;

namespace Terminal.Wpf.Controls;

/// <summary>
/// Provides a reusable WPF user control for hosting an interactive terminal session.
/// </summary>
public sealed class TerminalControl : UserControl
{
    private readonly Border _border;
    private readonly TerminalViewportControl _viewport;
    private bool _autoStartPending = true;

    /// <summary>
    /// Defines the session options property.
    /// </summary>
    public static readonly DependencyProperty SessionOptionsProperty = DependencyProperty.Register(
        nameof(SessionOptions),
        typeof(TerminalSessionOptions),
        typeof(TerminalControl),
        new PropertyMetadata(null, HandleSessionOptionsChanged));

    /// <summary>
    /// Defines the view model property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(TerminalControlViewModel),
        typeof(TerminalControl),
        new PropertyMetadata(null, HandleViewModelChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalControl"/> class.
    /// </summary>
    public TerminalControl()
    {
        ViewModel = new TerminalControlViewModel();
        _viewport = new TerminalViewportControl
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        _border = new Border
        {
            Background = Brushes.Black,
            BorderBrush = Brushes.DimGray,
            BorderThickness = new Thickness(1),
            Child = _viewport,
            Padding = new Thickness(4)
        };

        Content = _border;
        Focusable = true;
        DataContext = ViewModel;

        Loaded += HandleLoaded;
        Unloaded += HandleUnloaded;
        PreviewKeyDown += HandlePreviewKeyDown;
        TextInput += HandleTextInput;
        SizeChanged += HandleSizeChanged;
        PreviewMouseDown += HandlePreviewMouseDown;
        PreviewMouseUp += HandlePreviewMouseUp;
        PreviewMouseMove += HandlePreviewMouseMove;
        PreviewMouseWheel += HandlePreviewMouseWheel;
    }

    /// <summary>
    /// Gets or sets the startup options used for auto-started sessions.
    /// </summary>
    public TerminalSessionOptions? SessionOptions
    {
        get => (TerminalSessionOptions?)GetValue(SessionOptionsProperty);
        set => SetValue(SessionOptionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the terminal view model.
    /// </summary>
    public TerminalControlViewModel? ViewModel
    {
        get => (TerminalControlViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Starts the configured terminal session.
    /// </summary>
    public async Task StartAsync()
    {
        var viewModel = ViewModel;
        var options = SessionOptions;
        if (viewModel is null || options is null || viewModel.IsRunning)
        {
            return;
        }

        options.InitialSize = MeasureTerminalSize();
        await viewModel.StartAsync(options).ConfigureAwait(false);
        UpdateSnapshot(viewModel);
        _autoStartPending = false;
    }

    /// <summary>
    /// Stops the active terminal session.
    /// </summary>
    public Task StopAsync()
    {
        return ViewModel?.StopAsync() ?? Task.CompletedTask;
    }

    private static void HandleViewModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is TerminalControl terminalControl)
        {
            terminalControl.HandleViewModelChanged(e.OldValue as TerminalControlViewModel, e.NewValue as TerminalControlViewModel);
        }
    }

    private static void HandleSessionOptionsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is TerminalControl terminalControl)
        {
            terminalControl._autoStartPending = true;
        }
    }

    private void HandleViewModelChanged(TerminalControlViewModel? previousViewModel, TerminalControlViewModel? viewModel)
    {
        if (previousViewModel is not null)
        {
            previousViewModel.PropertyChanged -= HandleViewModelPropertyChanged;
        }

        DataContext = viewModel;
        if (viewModel is not null)
        {
            viewModel.PropertyChanged += HandleViewModelPropertyChanged;
            UpdateSnapshot(viewModel);
        }
        else
        {
            _viewport.Snapshot = null;
        }
    }

    private void HandleLoaded(object sender, RoutedEventArgs e)
    {
        if (_autoStartPending && SessionOptions is not null)
        {
            QueueBackgroundTask(StartAsync());
        }
    }

    private void HandleUnloaded(object sender, RoutedEventArgs e)
    {
        QueueBackgroundTask(StopAsync());
    }

    private void HandlePreviewKeyDown(object sender, KeyEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning)
        {
            return;
        }

        Task? operation = null;

        switch (e.Key)
        {
            case Key.Enter:
                operation = viewModel.SendEnterAsync();
                break;
            case Key.Back:
                operation = viewModel.SendBackspaceAsync();
                break;
            case Key.Up:
                operation = viewModel.SendArrowAsync(TerminalArrowKey.Up);
                break;
            case Key.Down:
                operation = viewModel.SendArrowAsync(TerminalArrowKey.Down);
                break;
            case Key.Left:
                operation = viewModel.SendArrowAsync(TerminalArrowKey.Left);
                break;
            case Key.Right:
                operation = viewModel.SendArrowAsync(TerminalArrowKey.Right);
                break;
        }

        if (operation is null)
        {
            return;
        }

        e.Handled = true;
        QueueBackgroundTask(operation);
    }

    private void HandleTextInput(object sender, TextCompositionEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        e.Handled = true;
        QueueBackgroundTask(viewModel.SendTextAsync(e.Text));
    }

    private void HandleSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null)
        {
            return;
        }

        QueueBackgroundTask(ResizeTerminalAsync(viewModel));
    }

    private void HandlePreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        Focus();

        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || viewModel.MouseProtocol != TerminalMouseProtocol.Sgr || viewModel.MouseTrackingMode == TerminalMouseTrackingMode.None)
        {
            return;
        }

        if (!TryCreateMousePayload(viewModel, e.GetPosition(_viewport), GetButtonCode(e.ChangedButton), isRelease: false, isMotion: false, out var payload))
        {
            return;
        }

        e.Handled = true;
        QueueBackgroundTask(viewModel.SendMouseAsync(payload));
    }

    private void HandlePreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || viewModel.MouseProtocol != TerminalMouseProtocol.Sgr || viewModel.MouseTrackingMode == TerminalMouseTrackingMode.None)
        {
            return;
        }

        if (!TryCreateMousePayload(viewModel, e.GetPosition(_viewport), 3, isRelease: true, isMotion: false, out var payload))
        {
            return;
        }

        e.Handled = true;
        QueueBackgroundTask(viewModel.SendMouseAsync(payload));
    }

    private void HandlePreviewMouseMove(object sender, MouseEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || viewModel.MouseProtocol != TerminalMouseProtocol.Sgr)
        {
            return;
        }

        var trackingMode = viewModel.MouseTrackingMode;
        if (trackingMode == TerminalMouseTrackingMode.None)
        {
            return;
        }

        var hasPressedButton = e.LeftButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;
        if (trackingMode == TerminalMouseTrackingMode.Drag && !hasPressedButton)
        {
            return;
        }

        if (trackingMode == TerminalMouseTrackingMode.Button)
        {
            return;
        }

        var buttonCode = GetMotionButtonCode(e);
        if (!TryCreateMousePayload(viewModel, e.GetPosition(_viewport), buttonCode, isRelease: false, isMotion: true, out var payload))
        {
            return;
        }

        e.Handled = true;
        QueueBackgroundTask(viewModel.SendMouseAsync(payload));
    }

    private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || viewModel.MouseProtocol != TerminalMouseProtocol.Sgr || viewModel.MouseTrackingMode == TerminalMouseTrackingMode.None)
        {
            return;
        }

        var buttonCode = e.Delta >= 0 ? 64 : 65;
        if (!TryCreateMousePayload(viewModel, e.GetPosition(_viewport), buttonCode, isRelease: false, isMotion: false, out var payload))
        {
            return;
        }

        e.Handled = true;
        QueueBackgroundTask(viewModel.SendMouseAsync(payload));
    }

    private async Task ResizeTerminalAsync(TerminalControlViewModel viewModel)
    {
        await viewModel.ResizeAsync(MeasureTerminalSize()).ConfigureAwait(false);
        UpdateSnapshot(viewModel);
    }

    private bool TryCreateMousePayload(TerminalControlViewModel viewModel, Point position, int buttonCode, bool isRelease, bool isMotion, out string payload)
    {
        payload = string.Empty;
        var snapshot = viewModel.Snapshot;
        if (snapshot is null)
        {
            return false;
        }

        var cellWidth = _viewport.GetCellWidth();
        var cellHeight = _viewport.GetCellHeight();
        var column = Math.Clamp((int)Math.Floor(position.X / cellWidth) + 1, 1, snapshot.Size.Columns);
        var row = Math.Clamp((int)Math.Floor(position.Y / cellHeight) + 1, 1, snapshot.Size.Rows);
        var effectiveButtonCode = isMotion ? buttonCode + 32 : buttonCode;
        payload = TerminalInputEncoder.EncodeMouseSgr(effectiveButtonCode, column, row, isRelease);
        return true;
    }

    private static int GetButtonCode(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => 0,
            MouseButton.Middle => 1,
            MouseButton.Right => 2,
            _ => 0
        };
    }

    private static int GetMotionButtonCode(MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            return 0;
        }

        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            return 1;
        }

        if (e.RightButton == MouseButtonState.Pressed)
        {
            return 2;
        }

        return 3;
    }

    private void HandleViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is TerminalControlViewModel viewModel && e.PropertyName == nameof(TerminalControlViewModel.Snapshot))
        {
            UpdateSnapshot(viewModel);
        }
    }

    private void UpdateSnapshot(TerminalControlViewModel viewModel)
    {
        Dispatcher.InvokeAsync(() =>
        {
            _viewport.Snapshot = viewModel.Snapshot;
            _viewport.InvalidateVisual();
        });
    }

    private static void QueueBackgroundTask(Task task)
    {
        if (task.IsCompletedSuccessfully)
        {
            return;
        }

        _ = ObserveBackgroundTaskAsync(task);
    }

    private static async Task ObserveBackgroundTaskAsync(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private TerminalSize MeasureTerminalSize()
    {
        var usableWidth = Math.Max(1d, ActualWidth - _border.Padding.Left - _border.Padding.Right - _border.BorderThickness.Left - _border.BorderThickness.Right);
        var usableHeight = Math.Max(1d, ActualHeight - _border.Padding.Top - _border.Padding.Bottom - _border.BorderThickness.Top - _border.BorderThickness.Bottom);
        var columns = Math.Max(1, (int)Math.Floor(usableWidth / _viewport.GetCellWidth()));
        var rows = Math.Max(1, (int)Math.Floor(usableHeight / _viewport.GetCellHeight()));
        return new TerminalSize(columns, rows);
    }
}
