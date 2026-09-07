using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Desktop.ViewModels;

/// <summary>Small keyboard-friendly ASCII character browser.</summary>
public partial class CharactersViewModel : ObservableObject
{
    private readonly IClipboardService? _clipboard;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Preview))]
    [NotifyPropertyChangedFor(nameof(TableText))]
    private int code = 32;

    public CharactersViewModel(IClipboardService? clipboard = null) =>
        _clipboard = clipboard;

    public string Preview =>
        $"Selected: {DisplayCharacter(Code)} U+{Code:X4}";

    public string TableText
    {
        get
        {
            var firstCode = 32 + ((Code - 32) / 32) * 32;
            return string.Join(
                Environment.NewLine,
                Enumerable.Range(0, 8)
                    .Select(row => new string(Enumerable.Range(0, 32)
                        .Select(column => (char)(firstCode + row * 32 + column))
                        .ToArray())));
        }
    }

    [RelayCommand]
    private void Previous() => Code = Code <= 32 ? 126 : Code - 1;

    [RelayCommand]
    private void Next() => Code = Code >= 126 ? 32 : Code + 1;

    [RelayCommand]
    private async Task CopyAsync(CancellationToken cancellationToken)
    {
        if (_clipboard is not null)
            await _clipboard.CopyAsync(((char)Code).ToString(), cancellationToken).ConfigureAwait(false);
    }

    private static string DisplayCharacter(int value) =>
        value == ' ' ? "space" : ((char)value).ToString();
}
