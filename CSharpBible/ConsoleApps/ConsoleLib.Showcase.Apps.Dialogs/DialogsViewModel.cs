using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Dialogs;

/// <summary>Interaction state for the reusable dialog showcase.</summary>
public partial class DialogsViewModel : ObservableObject
{
    private readonly IShowcaseFileDialogService _fileDialogs;

    [ObservableProperty]
    private string title = "ConsoleLib Dialogs";

    [ObservableProperty]
    private string status = "Choose a dialog.";

    public DialogsViewModel(IShowcaseFileDialogService? fileDialogs = null)
        => _fileDialogs = fileDialogs ?? new UnavailableShowcaseFileDialogService();

    public IShowcaseFileDialogService FileDialogs => _fileDialogs;

    [RelayCommand]
    private void OpenSettings() => Status = "Settings dialog ready.";

    [RelayCommand]
    private async Task OpenFile(CancellationToken cancellationToken)
        => Status = FormatResult(await _fileDialogs.OpenFileAsync(
            new FileDialogRequest("Open file"), cancellationToken));

    [RelayCommand]
    private async Task SaveFile(CancellationToken cancellationToken)
        => Status = FormatResult(await _fileDialogs.SaveFileAsync(
            new FileDialogRequest("Save file"), cancellationToken));

    [RelayCommand]
    private async Task PickFolder(CancellationToken cancellationToken)
        => Status = FormatResult(await _fileDialogs.PickFolderAsync(
            new FileDialogRequest("Pick folder"), cancellationToken));

    private static string FormatResult(FileDialogResult result)
    {
        if (!result.IsAvailable)
            return result.Message ?? "File dialogs are unavailable.";
        if (result.IsFailed)
            return result.Message ?? "File dialog failed.";
        return result.IsAccepted
            ? "Selected: " + result.Path
            : "Selection cancelled.";
    }
}
