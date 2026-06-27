using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avln_ImageEditor.Host.ViewModels;

namespace Avln_ImageEditor.Host.Views;

/// <summary>
/// Desktop shell used to test the reusable image editor control.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OpenImage_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        try
        {
            await viewModel.OpenImageAsync(StorageProvider);
        }
        catch (Exception ex)
        {
            viewModel.StatusText = ex.Message;
        }
    }
}
