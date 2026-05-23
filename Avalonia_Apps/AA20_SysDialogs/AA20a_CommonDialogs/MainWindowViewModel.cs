using System.Text;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln_CommonDialogs.Base.Interfaces;
using Avln_CommonDialogs.Base.Models;

namespace AA20a_CommonDialogs;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IOpenFileDialog _open;
    private readonly ISaveFileDialog _save;
    private readonly IColorDialog _color;
    private readonly IFontDialog _font;
    private readonly TopLevelAccessor _topLevelAccessor;

    public MainWindowViewModel(
        IOpenFileDialog open,
        ISaveFileDialog save,
        IColorDialog color,
        IFontDialog font,
        TopLevelAccessor topLevelAccessor)
    {
        _open = open;
        _save = save;
        _color = color;
        _font = font;
        _topLevelAccessor = topLevelAccessor;

        _open.Title = "Open";
        _open.AllowMultiple = true;
        (_open as dynamic).MutableFilters?.Add(new FileTypeFilter("Text", new[] { ".txt", ".md" }));

        _save.Title = "Save";
        _save.DefaultExtension = "txt";
        (_save as dynamic).MutableFilters?.Add(new FileTypeFilter("Text", new[] { ".txt" }));
    }

    [ObservableProperty]
    private string result = string.Empty;

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var files = await _open.ShowAsync();
        var sb = new StringBuilder();
        sb.AppendLine("OpenFile:");
        foreach (var f in files)
            sb.AppendLine(f);
        Result = sb.ToString();
    }

    [RelayCommand]
    private async Task SaveFileAsync()
    {
        var file = await _save.ShowAsync();
        Result = $"SaveFile: {file}";
    }

    [RelayCommand]
    private async Task ColorAsync()
    {
        var ok = await _color.ShowAsync();
        Result = $"ColorDialog: {ok}, Color={_color.Color}";
    }

    [RelayCommand]
    private async Task FontAsync()
    {
        ConfigureFontDialog(FontDialogPresentationMode.Window);

        var ok = await _font.ShowAsync();
        Result = $"FontDialog: {ok}, Font={_font.Font}, Selection={_font.Selection}";
    }

    [RelayCommand]
    private async Task FontOverlayAsync()
    {
        ConfigureFontDialog(FontDialogPresentationMode.Overlay);

        var ok = _topLevelAccessor.Current is { } topLevel
            ? await _font.ShowAsync(topLevel)
            : await _font.ShowAsync();

        Result = $"FontDialog Overlay: {ok}, Font={_font.Font}, Selection={_font.Selection}";
    }

    private void ConfigureFontDialog(FontDialogPresentationMode presentationMode)
    {
        _font.Selection = new FontDialogSelection
        {
            FamilyName = (_font.Font as FontFamily)?.Name,
            Size = 18,
            IsBold = true,
            ArgbColor = 0xFF1E3A8A
        };
        _font.PreviewText = "Avalonia FontPicker Beispieltext mit Umlauten: ÄÖÜ äöü ß";
        _font.PresentationMode = presentationMode;
    }
}
