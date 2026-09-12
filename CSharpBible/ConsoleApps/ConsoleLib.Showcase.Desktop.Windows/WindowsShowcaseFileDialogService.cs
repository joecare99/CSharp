using System;
using System.Threading;
using System.Threading.Tasks;
using CommonDialogs;
using CommonDialogs.Interfaces;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Desktop.Capabilities;

/// <summary>Windows adapter for the native CommonDialogs open, save, and folder dialogs.</summary>
public sealed class WindowsShowcaseFileDialogService : IShowcaseFileDialogService
{
    private readonly Func<IOpenFileDialog> _openDialog;
    private readonly Func<IFileDialog> _saveDialog;
    private readonly Func<IFileDialog> _folderDialog;

    public WindowsShowcaseFileDialogService()
        : this(() => new OpenFileDialogProxy(), () => new SaveFileDialogProxy(), () => new CommonDialogs.FolderBrowserDialog())
    {
    }

    public WindowsShowcaseFileDialogService(
        Func<IOpenFileDialog> openDialog,
        Func<IFileDialog> saveDialog,
        Func<IFileDialog> folderDialog)
    {
        _openDialog = openDialog ?? throw new ArgumentNullException(nameof(openDialog));
        _saveDialog = saveDialog ?? throw new ArgumentNullException(nameof(saveDialog));
        _folderDialog = folderDialog ?? throw new ArgumentNullException(nameof(folderDialog));
    }

    public Task<FileDialogResult> OpenFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
        ShowAsync(request, cancellationToken, _openDialog, ConfigureFile);

    public Task<FileDialogResult> SaveFileAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
        ShowAsync(request, cancellationToken, _saveDialog, ConfigureFile);

    public Task<FileDialogResult> PickFolderAsync(FileDialogRequest request, CancellationToken cancellationToken = default) =>
        ShowAsync(request, cancellationToken, _folderDialog, ConfigureFile);

    private static Task<FileDialogResult> ShowAsync<T>(
        FileDialogRequest request,
        CancellationToken cancellationToken,
        Func<T> factory,
        Action<T, FileDialogRequest> configure)
        where T : IFileDialog
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var dialog = factory() ?? throw new InvalidOperationException("The dialog factory returned no dialog.");
            configure(dialog, request);
            cancellationToken.ThrowIfCancellationRequested();
            var accepted = dialog.ShowDialog() == true;
            return Task.FromResult(accepted
                ? FileDialogResult.Accepted(dialog.FileName)
                : FileDialogResult.Cancelled());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return Task.FromResult(FileDialogResult.Failed(exception.Message));
        }
    }

    private static void ConfigureFile(IFileDialog dialog, FileDialogRequest request)
    {
        dialog.Title = request.Title;
        if (!string.IsNullOrWhiteSpace(request.InitialPath))
            dialog.InitialDirectory = request.InitialPath;
        if (!string.IsNullOrWhiteSpace(request.Filter))
            dialog.Filter = request.Filter;
    }
}
