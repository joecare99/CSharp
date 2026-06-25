using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Terminal.Avalonia.ViewModels;
using Terminal.Core;

namespace Terminal.Avalonia.Controls;

/// <summary>
/// Provides a reusable Avalonia user control for hosting an interactive terminal session.
/// </summary>
public sealed class TerminalControl : UserControl
{
    private readonly Border _border;
    private readonly TerminalViewportControl _viewport;
    private bool _autoStartPending = true;

    /// <summary>
    /// Defines the session options property.
    /// </summary>
    public static readonly StyledProperty<TerminalSessionOptions?> SessionOptionsProperty =
        AvaloniaProperty.Register<TerminalControl, TerminalSessionOptions?>(nameof(SessionOptions));

    /// <summary>
    /// Defines the view model property.
    /// </summary>
    public static readonly StyledProperty<TerminalControlViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<TerminalControl, TerminalControlViewModel?>(nameof(ViewModel));

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

        AttachedToVisualTree += HandleAttachedToVisualTree;
        DetachedFromVisualTree += HandleDetachedFromVisualTree;
        KeyDown += HandleKeyDown;
        TextInput += HandleTextInput;
        SizeChanged += HandleSizeChanged;
        PointerPressed += HandlePointerPressed;
        PointerReleased += HandlePointerReleased;
        PointerMoved += HandlePointerMoved;
        PointerWheelChanged += HandlePointerWheelChanged;
    }

    /// <summary>
    /// Gets or sets the startup options used for auto-started sessions.
    /// </summary>
    public TerminalSessionOptions? SessionOptions
    {
        get => GetValue(SessionOptionsProperty);
        set => SetValue(SessionOptionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the terminal view model.
    /// </summary>
    public TerminalControlViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
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

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ViewModelProperty)
        {
            HandleViewModelChanged(change.NewValue as TerminalControlViewModel);
        }
        else if (change.Property == SessionOptionsProperty)
        {
            _autoStartPending = true;
        }
    }

    private void HandleViewModelChanged(TerminalControlViewModel? viewModel)
    {
        if (DataContext is TerminalControlViewModel previousViewModel)
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

    private async void HandleAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_autoStartPending && SessionOptions is not null)
        {
            await StartAsync().ConfigureAwait(false);
        }
    }

    private async void HandleDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        await StopAsync().ConfigureAwait(false);
    }

    private async void HandleKeyDown(object? sender, KeyEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Enter:
                await viewModel.SendEnterAsync().ConfigureAwait(false);
                e.Handled = true;
                break;
            case Key.Back:
                await viewModel.SendBackspaceAsync().ConfigureAwait(false);
                e.Handled = true;
                break;
            case Key.Up:
                await viewModel.SendArrowAsync(TerminalArrowKey.Up).ConfigureAwait(false);
                e.Handled = true;
                break;
            case Key.Down:
                await viewModel.SendArrowAsync(TerminalArrowKey.Down).ConfigureAwait(false);
                e.Handled = true;
                break;
            case Key.Left:
                await viewModel.SendArrowAsync(TerminalArrowKey.Left).ConfigureAwait(false);
                e.Handled = true;
                break;
            case Key.Right:
                await viewModel.SendArrowAsync(TerminalArrowKey.Right).ConfigureAwait(false);
                e.Handled = true;
                break;
        }
    }

    private async void HandleTextInput(object? sender, TextInputEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        await viewModel.SendTextAsync(e.Text).ConfigureAwait(false);
        e.Handled = true;
    }

    private async void HandleSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null)
        {
            return;
        }

        await viewModel.ResizeAsync(MeasureTerminalSize()).ConfigureAwait(false);
        UpdateSnapshot(viewModel);
    }

    private async void HandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Focus();

        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || viewModel.MouseProtocol != TerminalMouseProtocol.Sgr || viewModel.MouseTrackingMode == TerminalMouseTrackingMode.None)
        {
            return;
        }

        if (!TryCreateMousePayload(viewModel, e.GetPosition(_viewport), GetButtonCode(e.GetCurrentPoint(_viewport).Properties), isRelease: false, isMotion: false, out var payload))
        {
            return;
        }

        await viewModel.SendMouseAsync(payload).ConfigureAwait(false);
        e.Handled = true;
    }

    private async void HandlePointerReleased(object? sender, PointerReleasedEventArgs e)
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

        await viewModel.SendMouseAsync(payload).ConfigureAwait(false);
        e.Handled = true;
    }

    private async void HandlePointerMoved(object? sender, PointerEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || viewModel.MouseProtocol != TerminalMouseProtocol.Sgr)
        {
            return;
        }

        var trackingMode = viewModel.MouseTrackingMode;
        if (trackingMode == TerminalMouseTrackingMode.None || trackingMode == TerminalMouseTrackingMode.Button)
        {
            return;
        }

        var properties = e.GetCurrentPoint(_viewport).Properties;
        var hasPressedButton = properties.IsLeftButtonPressed || properties.IsMiddleButtonPressed || properties.IsRightButtonPressed;
        if (trackingMode == TerminalMouseTrackingMode.Drag && !hasPressedButton)
        {
            return;
        }

        var buttonCode = GetMotionButtonCode(properties);
        if (!TryCreateMousePayload(viewModel, e.GetPosition(_viewport), buttonCode, isRelease: false, isMotion: true, out var payload))
        {
            return;
        }

        await viewModel.SendMouseAsync(payload).ConfigureAwait(false);
        e.Handled = true;
    }

    private async void HandlePointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel is null || !viewModel.IsRunning || viewModel.MouseProtocol != TerminalMouseProtocol.Sgr || viewModel.MouseTrackingMode == TerminalMouseTrackingMode.None)
        {
            return;
        }

        var buttonCode = e.Delta.Y >= 0 ? 64 : 65;
        if (!TryCreateMousePayload(viewModel, e.GetPosition(_viewport), buttonCode, isRelease: false, isMotion: false, out var payload))
        {
            return;
        }

        await viewModel.SendMouseAsync(payload).ConfigureAwait(false);
        e.Handled = true;
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
        Dispatcher.UIThread.Post(() =>
        {
            _viewport.Snapshot = viewModel.Snapshot;
            _viewport.InvalidateVisual();
        });
    }

    private TerminalSize MeasureTerminalSize()
    {
        var usableWidth = Math.Max(1d, Bounds.Width - _border.Padding.Left - _border.Padding.Right - 2);
        var usableHeight = Math.Max(1d, Bounds.Height - _border.Padding.Top - _border.Padding.Bottom - 2);
        var columns = Math.Max(1, (int)Math.Floor(usableWidth / _viewport.GetCellWidth()));
        var rows = Math.Max(1, (int)Math.Floor(usableHeight / _viewport.GetCellHeight()));
        return new TerminalSize(columns, rows);
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

    private static int GetButtonCode(PointerPointProperties properties)
    {
        return properties.PointerUpdateKind switch
        {
            PointerUpdateKind.LeftButtonPressed => 0,
            PointerUpdateKind.MiddleButtonPressed => 1,
            PointerUpdateKind.RightButtonPressed => 2,
            _ => 0
        };
    }

    private static int GetMotionButtonCode(PointerPointProperties properties)
    {
        if (properties.IsLeftButtonPressed)
        {
            return 0;
        }

        if (properties.IsMiddleButtonPressed)
        {
            return 1;
        }

        if (properties.IsRightButtonPressed)
        {
            return 2;
        }

        return 3;
    }
}
