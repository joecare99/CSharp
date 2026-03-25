using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln_CommonDialogs.Base.Interfaces;

namespace AA20a_CommonDialogs;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IOpenFileDialog _open;
    private readonly ISaveFileDialog _save;
    private readonly IColorDialog _color;
    private readonly IFontDialog _font;

    public MainWindowViewModel(
        IOpenFileDialog open,
        ISaveFileDialog save,
        IColorDialog color,
        IFontDialog font)
    {
        _open = open;
        _save = save;
        _color = color;
        _font = font;

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
        var ok = await _font.ShowAsync();
        Result = $"FontDialog: {ok}, Font={_font.Font}";
    }
}
