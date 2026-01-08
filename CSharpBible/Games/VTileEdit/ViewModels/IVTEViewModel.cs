using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using VTileEdit.Models;

namespace VTileEdit.ViewModels;

public interface IVTEViewModel : INotifyPropertyChanged
{
    IRelayCommand NewTilesCommand { get; }
    IRelayCommand<string?> LoadTilesCommand { get; }
    IRelayCommand<string?> SaveTilesCommand { get; }
    IRelayCommand<string?> SaveTileCommand { get; }
    IRelayCommand SelectTileCommand { get; }
    IRelayCommand EditTileCommand { get; }
    IRelayCommand QuitCommand { get; }
    int? SelectedTile { get; set; }
    string[] CurrentLines { get; set; }
    FullColor[] CurrentColors { get; set; }
    Func<Size> DoNewTileDialog { get; set; }
    Size TileSize { get; }
    ReadOnlyCollection<char> CharacterPalette { get; }

    void SaveToPath(string path);
    void SaveTileToPath(int tile, string path);
    void UpdateCurrentTile(string[] lines, FullColor[] colors);
}