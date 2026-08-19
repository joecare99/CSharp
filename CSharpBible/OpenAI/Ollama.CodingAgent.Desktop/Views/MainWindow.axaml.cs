using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ollama.CodingAgent.Desktop.ViewModels;

namespace Ollama.CodingAgent.Desktop.Views;

/// <summary>
/// Hosts the focused desktop sections for one shared coding-agent session.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes the window for Avalonia's runtime XAML loader.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes the main window with its desktop presentation adapter.
    /// </summary>
    public MainWindow(DesktopSessionViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
        OpenWorkspaceDirectoryButton.Click += OpenWorkspaceDirectoryClicked;
    }

    private async void OpenWorkspaceDirectoryClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DesktopSessionViewModel viewModel)
        {
            return;
        }

        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Select workspace directory",
            });
        IStorageFolder? folder = folders.Count == 0 ? null : folders[0];
        string? path = folder?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path))
        {
            viewModel.EditableWorkspacePath = path;
        }
    }
}
