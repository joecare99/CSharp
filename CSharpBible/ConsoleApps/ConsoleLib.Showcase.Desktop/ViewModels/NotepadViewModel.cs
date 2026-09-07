using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Desktop.ViewModels;

/// <summary>Portable notepad state using the host-provided clipboard contract.</summary>
public partial class NotepadViewModel : ObservableObject
{
    private readonly IClipboardService? _clipboard;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Statistics))]
    private string document = string.Empty;

    public NotepadViewModel(IClipboardService? clipboard = null) =>
        _clipboard = clipboard;

    public string Statistics =>
        $"{Document.Length} characters, {CountLines(Document)} lines";

    public string ClipboardStatus =>
        _clipboard is null ? "Clipboard unavailable on this host." : "Clipboard ready.";

    [RelayCommand]
    private void NewDocument() => Document = string.Empty;

    [RelayCommand]
    private async Task CopyAsync(CancellationToken cancellationToken)
    {
        if (_clipboard is not null)
            await _clipboard.CopyAsync(Document, cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task PasteAsync(CancellationToken cancellationToken)
    {
        if (_clipboard is not null)
        {
            var text = await _clipboard.PasteAsync(cancellationToken).ConfigureAwait(false);
            if (text is not null)
                Document += text;
        }
    }

    private static int CountLines(string text) =>
        string.IsNullOrEmpty(text) ? 0 : text.Split('\n').Length;
}
