using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.DevOpsPlanning.Host.Services;

/// <summary>
/// Implements folder selection using the current Avalonia desktop storage provider.
/// </summary>
public sealed class AvaloniaDevOpsFolderPickerService : IDevOpsFolderPickerService
{
    /// <inheritdoc/>
    public async Task<string?> PickFolderAsync(string title, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            return null;
        }

        Window? mainWindow = desktop.MainWindow;
        if (mainWindow?.StorageProvider is not { } storageProvider)
        {
            return null;
        }

        IReadOnlyList<IStorageFolder> selectedFolders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
        });

        if (selectedFolders.Count == 0)
        {
            return null;
        }

        return selectedFolders[0].Path.LocalPath;
    }
}
