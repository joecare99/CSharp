using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using VTileEdit.Models;
using VTileEdit.ViewModels;

namespace VTileEdit;

public partial class VTEViewModel : ObservableObject, IVTEViewModel, INotifyPropertyChanged
{
    private IVTEModel _model;

    public VTEViewModel(IVTEModel model)
    {
        _model = model;
    }

    public IVTEModel Model => _model;

    [ObservableProperty]
    public partial Enum? SelectedTile { get; set; }

    [ObservableProperty]
    public partial string[] CurrentLines { get; set; } = Array.Empty<string>();

    [ObservableProperty]
    public partial FullColor[] CurrentColors { get; set; } = Array.Empty<FullColor>();
    public Func<Size> DoNewTileDialog { get; set; }

    public Size TileSize => _model.TileSize;

    public void LoadFromPath(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        switch (Path.GetExtension(path).ToLowerInvariant())
        {
            case ".txt": _model.LoadFromStream(fs, EStreamType.Text); break;
            case ".tdf": _model.LoadFromStream(fs, EStreamType.Binary); break;
            case ".tdj": _model.LoadFromStream(fs, EStreamType.Json); break;
            case ".tdx": _model.LoadFromStream(fs, EStreamType.Xml); break;
            default: throw new NotSupportedException("Unsupported file extension");
        }
        SelectedTile = null; CurrentLines = Array.Empty<string>(); CurrentColors = Array.Empty<FullColor>();
    }

    public void SaveToPath(string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        switch (Path.GetExtension(path).ToLowerInvariant())
        {
            case ".txt": _model.SaveToStream(fs, EStreamType.Text); break;
            case ".tdf": _model.SaveToStream(fs, EStreamType.Binary); break;
            case ".tdj": _model.SaveToStream(fs, EStreamType.Json); break;
            case ".tdx": _model.SaveToStream(fs, EStreamType.Xml); break;
            case ".cs": _model.SaveToStream(fs, EStreamType.Code); break;
            default: throw new NotSupportedException("Unsupported file extension");
        }
    }

    public void SaveTileToPath(Enum tile, string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        switch (Path.GetExtension(path).ToLowerInvariant())
        {
            case ".txt": _model.SaveTileToStream(tile, fs, EStreamType.Text); break;
            case ".tdf": _model.SaveTileToStream(tile, fs, EStreamType.Binary); break;
            case ".tdj": _model.SaveTileToStream(tile, fs, EStreamType.Json); break;
            case ".tdx": _model.SaveTileToStream(tile, fs, EStreamType.Xml); break;
            case ".cs": _model.SaveTileToStream(tile, fs, EStreamType.Code); break;
            default: throw new NotSupportedException("Unsupported file extension");
        }
    }

    public void SelectTile(Enum tile)
    {
        SelectedTile = tile;
        var st = _model.GetTileDef(tile);
        CurrentLines = st.lines;
        CurrentColors = st.colors;
    }

    public void UpdateCurrentTile(string[] lines, FullColor[] colors)
    {
        if (SelectedTile != null)
        {
            _model.SetTileDef(SelectedTile, lines, colors);
            CurrentLines = lines;
            CurrentColors = colors;
        }
    }

    [RelayCommand]
    private void NewTiles()
    {
        _model.Clear();
        var size = DoNewTileDialog?.Invoke() ?? new Size(4, 2);
        SelectedTile = null;
        _model.SetTileSize(size);
    }

    [RelayCommand]
    private void LoadTiles(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        LoadFromPath(path);

        CurrentLines = Array.Empty<string>();
        CurrentColors = Array.Empty<FullColor>();
    }

    [RelayCommand]
    private void SaveTiles(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        SaveToPath(path);
    }

    [RelayCommand]
    private void SaveTile(string? path)
    {
        if (SelectedTile == null || string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        SaveTileToPath(SelectedTile, path);
    }

    [RelayCommand]
    private void Quit()
    {
        // Handle quitting the application
    }

    [RelayCommand]
    private void About()
    {
        // Show about dialog
    }
    [RelayCommand]
    private void SelectTile()
    {
        // Show select-tile dialog
    }

    [RelayCommand]
    private void EditTile()
    {
        // Show edit-colors dialog
    }
}

public static class FileDialogFilter
{
    public static string Build(IEnumerable<(string, IEnumerable<string>)> filters)
    {
        var sb = new StringBuilder();
        foreach (var filter in filters)
        {
            sb.Append(filter.Item1);
            sb.Append("|");
            sb.Append(string.Join(";", filter.Item2.Select(e => $"*.{e}")));
            sb.Append("|");
        }
        sb.Append("|");
        return sb.ToString();
    }
}
