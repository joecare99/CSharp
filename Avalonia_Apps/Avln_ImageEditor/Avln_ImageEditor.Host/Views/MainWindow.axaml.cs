using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avln_ImageEditor.Host.ViewModels;

namespace Avln_ImageEditor.Host.Views;

public partial class MainWindow : Window
{
    private bool _confirmedClose;
    private bool _askingToClose;
    private bool _fileOperation;

    public MainWindow()
    {
        InitializeComponent();
        Closing += OnClosing;
    }

    private async void OpenImage_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        if (_fileOperation) return;
        _fileOperation = true;
        try { await vm.OpenImageAsync(StorageProvider, ConfirmDiscardAsync); }
        catch (Exception ex) { vm.StatusText = $"Open failed: {ex.Message}"; }
        finally { _fileOperation = false; }
    }

    private async void ExportPng_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        if (_fileOperation) return;
        _fileOperation = true;
        try { await vm.ExportPngAsync(StorageProvider); }
        catch (Exception ex) { vm.StatusText = $"Export failed: {ex.Message}"; }
        finally { _fileOperation = false; }
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_confirmedClose || DataContext is not MainWindowViewModel vm) return;
        if (_fileOperation)
        {
            e.Cancel = true;
            vm.StatusText = "Wait for the file operation to finish before closing.";
            return;
        }
        if (!vm.Editor.IsDirty) { vm.Dispose(); return; }
        e.Cancel = true;
        if (_askingToClose) return;
        _askingToClose = true;
        try
        {
            if (await ConfirmDiscardAsync())
            {
                _confirmedClose = true;
                vm.Dispose();
                Close();
            }
        }
        finally { _askingToClose = false; }
    }

    private async Task<bool> ConfirmDiscardAsync()
    {
        var discard = new Button { Content = "Discard changes" };
        var cancel = new Button { Content = "Cancel" };
        var dialog = new Window
        {
            Title = "Unsaved image changes",
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = "Discard unsaved image changes?" },
                    new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { discard, cancel } }
                }
            }
        };
        discard.Click += (_, _) => dialog.Close(true);
        cancel.Click += (_, _) => dialog.Close(false);
        return await dialog.ShowDialog<bool>(this);
    }
}
