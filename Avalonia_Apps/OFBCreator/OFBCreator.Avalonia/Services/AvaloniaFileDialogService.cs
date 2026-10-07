using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace OFBCreator.Avalonia.Services;

/// <summary>
/// Implements workspace file selection using the active Avalonia desktop window's storage provider.
/// </summary>
public sealed class AvaloniaFileDialogService : IOFBFileDialogService
{
    private static readonly FilePickerFileType ProjectFileType = new("OFB project")
    {
        Patterns = ["*.ofbproject"]
    };

    private static readonly FilePickerFileType GedcomFileType = new("GEDCOM files")
    {
        Patterns = ["*.ged", "*.gedcom"]
    };

    private static readonly FilePickerFileType DocxFileType = new("Word document")
    {
        Patterns = ["*.docx"]
    };

    public async Task<string?> OpenProjectAsync(string? initialPath)
    {
        var result = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open OFB project",
            AllowMultiple = false,
            FileTypeFilter = [ProjectFileType],
            SuggestedStartLocation = await GetFolderAsync(initialPath)
        });
        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> SaveProjectAsAsync(string suggestedPath)
    {
        var result = await GetStorageProvider().SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save OFB project as",
            SuggestedFileName = Path.GetFileName(suggestedPath),
            DefaultExtension = "ofbproject",
            FileTypeChoices = [ProjectFileType],
            SuggestedStartLocation = await GetFolderAsync(Path.GetDirectoryName(suggestedPath))
        });
        return result?.Path.LocalPath;
    }

    public async Task<string?> PickDirectoryAsync(string? initialPath)
    {
        var result = await GetStorageProvider().OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select folder",
            AllowMultiple = false,
            SuggestedStartLocation = await GetFolderAsync(initialPath)
        });
        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> OpenGedcomAsync(string? initialPath)
    {
        var result = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open GEDCOM file",
            AllowMultiple = false,
            FileTypeFilter = [GedcomFileType],
            SuggestedStartLocation = await GetFolderAsync(initialPath)
        });
        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> SaveDocxAsAsync(string suggestedPath)
    {
        var result = await GetStorageProvider().SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save document as",
            SuggestedFileName = Path.GetFileName(suggestedPath),
            DefaultExtension = "docx",
            FileTypeChoices = [DocxFileType],
            SuggestedStartLocation = await GetFolderAsync(Path.GetDirectoryName(suggestedPath))
        });
        return result?.Path.LocalPath;
    }

    private static IStorageProvider GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is { } mainWindow)
            return mainWindow.StorageProvider;

        throw new InvalidOperationException("The Avalonia desktop window is not available for file dialogs.");
    }

    private static async Task<IStorageFolder?> GetFolderAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        var folderPath = Directory.Exists(path) ? path : Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            return null;

        return await GetStorageProvider().TryGetFolderFromPathAsync(new Uri(Path.GetFullPath(folderPath)));
    }
}
