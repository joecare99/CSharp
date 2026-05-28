using System;
using System.Threading.Tasks;
using AA09_DialogBoxes.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;

namespace AA09_DialogBoxes.Views;

public partial class BrowserEditDialogControl : UserControl
{
    private TaskCompletionSource<(bool, (string, string))>? _dialogCompletion;

    public BrowserEditDialogControl()
    {
        InitializeComponent();
        DataContext = new DialogWindowViewModel();
        KeyDown += BrowserEditDialogControl_KeyDown;
    }

    public Task<(bool, (string, string))> ShowAsync(string name, string email)
    {
        if (_dialogCompletion is { Task.IsCompleted: false })
            return _dialogCompletion.Task;

        _dialogCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        if (DataContext is DialogWindowViewModel vm)
        {
            vm.Name = name;
            vm.Email = email;
        }

        UpdateBackdrop(ActualThemeVariant);
        IsVisible = true;
        CenterOverlayDialog(Bounds);
        NameTextBox.Focus();
        return _dialogCompletion.Task;
    }

    private void BrowserEditDialogControl_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!IsVisible || _dialogCompletion is null)
            return;

        switch (e.Key)
        {
            case Key.Enter:
                Finish(true);
                e.Handled = true;
                break;
            case Key.Escape:
                Finish(false);
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

    private void Finish(bool accepted)
    {
        IsVisible = false;
        var result = (string.Empty, string.Empty);
        if (accepted && DataContext is DialogWindowViewModel vm)
            result = (vm.Name, vm.Email);

        _dialogCompletion?.TrySetResult((accepted, result));
        _dialogCompletion = null;
    }

    private void OverlayOk_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Finish(true);
    private void OverlayCancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Finish(false);
    private void OverlayClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Finish(false);
}
