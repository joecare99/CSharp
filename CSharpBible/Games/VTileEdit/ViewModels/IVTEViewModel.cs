using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Drawing;
using VTileEdit.Models;

namespace VTileEdit.ViewModels;

public interface IVTEViewModel : INotifyPropertyChanged
{
    IRelayCommand NewTilesCommand { get; }
    IRelayCommand LoadTilesCommand { get; }
    IRelayCommand SaveTilesCommand { get; }
    IRelayCommand SelectTileCommand { get; }
    IRelayCommand EditTileCommand { get; }
    IRelayCommand QuitCommand { get; }
    Enum? SelectedTile { get; set; }
    string[] CurrentLines { get; set; }
    FullColor[] CurrentColors { get; set; }
    Func<Size> DoNewTileDialog { get; set; }
    Size TileSize { get; }

    void SaveToPath(string path);
    void UpdateCurrentTile(string[] lines, FullColor[] colors);
}