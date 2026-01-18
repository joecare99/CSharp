using System.Windows;
using SharpHack.ViewModel;
using SharpHack.Wpf.Services;

namespace SharpHack.Wpf;

public partial class MainWindow : Window
{
    private readonly GameViewModel _viewModel;

    public ITileService TileService { get; }

    public MainWindow(GameViewModel viewModel, ITileService tileService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        TileService = tileService;

        TileService.LoadTileset("tiles2.png", tileSize: 96);

        DataContext = _viewModel;
    }
}
