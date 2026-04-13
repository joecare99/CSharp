using System;
using System.Threading.Tasks;
using AA09_DialogBoxes.Messages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;

namespace AA09_DialogBoxes.Views;

public partial class OverlayDialogControl : UserControl
{
    private TaskCompletionSource<MsgBoxResult>? _overlayCompletion;
    private bool _isDraggingOverlay;
    private Point _dragOffset;

    public OverlayDialogControl()
    {
        InitializeComponent();
        KeyDown += OverlayDialogControl_KeyDown;
    }

    public Task<MsgBoxResult> ShowAsync(Window owner, string title, string content)
    {
        if (_overlayCompletion is { Task.IsCompleted: false })
            return _overlayCompletion.Task;

        _overlayCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        OverlayTitle.Text = title;
        OverlayContent.Text = content;
        UpdateBackdrop(owner.ActualThemeVariant);
        IsVisible = true;
        CenterOverlayDialog(owner.Bounds);
        Focus();

        return _overlayCompletion.Task;
    }

    private void OverlayDialogControl_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!IsVisible || _overlayCompletion is null)
            return;

        switch (e.Key)
        {
            case Key.J:
            case Key.Enter:
                Finish(MsgBoxResult.Yes);
                e.Handled = true;
                break;
            case Key.N:
            case Key.Escape:
                Finish(MsgBoxResult.No);
                e.Handled = true;
                break;
        }
    }

    private void UpdateBackdrop(ThemeVariant theme)
    {
        OverlayBackdrop.Background = theme == ThemeVariant.Dark
            ? new SolidColorBrush(Color.Parse("#88000000"))
            : new SolidColorBrush(Color.Parse("#88FFFFFF"));
    }

    private void CenterOverlayDialog(Rect ownerBounds)
    {
        var dialogWidth = OverlayDialog.Bounds.Width > 0 ? OverlayDialog.Bounds.Width : OverlayDialog.Width;
        var dialogHeight = OverlayDialog.Bounds.Height > 0 ? OverlayDialog.Bounds.Height : OverlayDialog.Height;

        var left = Math.Max(0, (ownerBounds.Width - dialogWidth) / 2d);
        var top = Math.Max(0, (ownerBounds.Height - dialogHeight) / 2d);
        Canvas.SetLeft(OverlayDialog, left);
        Canvas.SetTop(OverlayDialog, top);
    }

    private void Finish(MsgBoxResult result)
    {
        IsVisible = false;
        _overlayCompletion?.TrySetResult(result);
        _overlayCompletion = null;
    }

    private void OverlayYes_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => Finish(MsgBoxResult.Yes);

    private void OverlayNo_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => Finish(MsgBoxResult.No);

    private void OverlayClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => Finish(MsgBoxResult.No);

    private void OverlayDialog_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
            return;

        _isDraggingOverlay = true;
        var pointerPos = e.GetPosition(OverlayCanvas);
        var left = Canvas.GetLeft(OverlayDialog);
        var top = Canvas.GetTop(OverlayDialog);
        if (double.IsNaN(left)) left = 0;
        if (double.IsNaN(top)) top = 0;
        _dragOffset = new Point(pointerPos.X - left, pointerPos.Y - top);
        e.Pointer.Capture(control);
    }

    private void OverlayDialog_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDraggingOverlay)
            return;

        var pointerPos = e.GetPosition(OverlayCanvas);
        var newLeft = pointerPos.X - _dragOffset.X;
        var newTop = pointerPos.Y - _dragOffset.Y;

        var maxLeft = Math.Max(0, OverlayCanvas.Bounds.Width - OverlayDialog.Bounds.Width);
        var maxTop = Math.Max(0, OverlayCanvas.Bounds.Height - OverlayDialog.Bounds.Height);

        Canvas.SetLeft(OverlayDialog, Math.Clamp(newLeft, 0, maxLeft));
        Canvas.SetTop(OverlayDialog, Math.Clamp(newTop, 0, maxTop));
    }

    private void OverlayDialog_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDraggingOverlay = false;
        e.Pointer.Capture(null);
    }
}
