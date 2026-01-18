using SharpHack.WPF2D.ViewModels;
using SharpHack.Wpf.Services;
using System.Windows;

namespace SharpHack.WPF2D;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow : Window
{
    public ITileService TileService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="vm">The view model.</param>
    public MainWindow(MainViewModel vm, ITileService tileService)
    {
        InitializeComponent();
        TileService = tileService;
        TileService.LoadTileset("tileset_cutout3.png", tileSize: 96);
        DataContext = vm;
    }
}
