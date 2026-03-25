using System;
using SharpHack.WPF2D.ViewModels;
using SharpHack.WPF2D.Converters;
using SharpHack.Base.Model;
using SharpHack.Wpf.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpHack.WPF2D;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public ITileService TileService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="vm">The view model.</param>
    public MainWindow(MainViewModel vm, ITileService tileService)
    {
        InitializeComponent();
        _viewModel = vm;
        TileService = tileService;
        TileService.LoadTileset("tileset_cutout3.png", tileSize: 96);

        if (Resources["TileToImageConverter"] is TileToImageConverter converter)
        {
            converter.TileService = TileService;
        }

        if (Resources["TopEntityTileConverter"] is TopEntityTileConverter topEntityConverter)
        {
            topEntityConverter.TileService = TileService;
        }

        DataContext = vm;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        Focus();
        Keyboard.Focus(this);
    }

    private void OnWindowPreviewKeyDown(object sender, KeyEventArgs e)
    {
        bool handled = e.Key switch
        {
            Key.Up => TryMove(Direction.North),
            Key.Down => TryMove(Direction.South),
            Key.Left => TryMove(Direction.West),
            Key.Right => TryMove(Direction.East),
            Key.NumPad7 => TryMove(Direction.NorthWest),
            Key.NumPad9 => TryMove(Direction.NorthEast),
            Key.NumPad1 => TryMove(Direction.SouthWest),
            Key.NumPad3 => TryMove(Direction.SouthEast),
            Key.Space => TryExecutePrimaryAction(),
            Key.Enter => TryExecutePrimaryAction(),
            _ => false
        };

        if (handled)
        {
            e.Handled = true;
        }
    }

    private async void OnMapMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ItemsControl itemsControl)
        {
            return;
        }

        if (_viewModel.Game.ViewWidth <= 0 || _viewModel.Game.ViewHeight <= 0 || itemsControl.ActualWidth <= 0 || itemsControl.ActualHeight <= 0)
        {
            return;
        }

        var position = e.GetPosition(itemsControl);
        double cellWidth = itemsControl.ActualWidth / _viewModel.Game.ViewWidth;
        double cellHeight = itemsControl.ActualHeight / _viewModel.Game.ViewHeight;

        if (cellWidth <= 0 || cellHeight <= 0)
        {
            return;
        }

        int viewX = Math.Clamp((int)(position.X / cellWidth), 0, _viewModel.Game.ViewWidth - 1);
        int viewY = Math.Clamp((int)(position.Y / cellHeight), 0, _viewModel.Game.ViewHeight - 1);

        var targetView = new SharpHack.Base.Model.Point(viewX, viewY);
        var action = _viewModel.Game.GetHoverActionForView(targetView);

        if (action == SharpHack.ViewModel.LayeredTileHoverAction.ToggleDoor)
        {
            _viewModel.Game.ToggleDoorAtView(targetView);
        }
        else if (action == SharpHack.ViewModel.LayeredTileHoverAction.Pickup)
        {
            _viewModel.Game.PickupAtView(targetView);
        }
        else if (action == SharpHack.ViewModel.LayeredTileHoverAction.GoTo)
        {
            await _viewModel.Game.GoToViewAsync(targetView);
        }

        Focus();
        Keyboard.Focus(this);
        e.Handled = true;
    }

    private void OnMapMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not ItemsControl itemsControl)
        {
            Mouse.OverrideCursor = null;
            return;
        }

        if (!TryGetViewPoint(itemsControl, e.GetPosition(itemsControl), out var targetView))
        {
            Mouse.OverrideCursor = null;
            return;
        }

        var action = _viewModel.Game.GetHoverActionForView(targetView);
        Mouse.OverrideCursor = action switch
        {
            SharpHack.ViewModel.LayeredTileHoverAction.ToggleDoor => Cursors.SizeWE,
            SharpHack.ViewModel.LayeredTileHoverAction.GoTo => Cursors.Hand,
            SharpHack.ViewModel.LayeredTileHoverAction.Pickup => Cursors.Hand,
            _ => Cursors.No
        };
    }

    private void OnMapMouseLeave(object sender, MouseEventArgs e)
    {
        Mouse.OverrideCursor = null;
    }

    private bool TryMove(Direction direction)
    {
        if (!_viewModel.Game.MoveCommand.CanExecute(direction))
        {
            return false;
        }

        _viewModel.Game.MoveCommand.Execute(direction);
        return true;
    }

    private bool TryExecutePrimaryAction()
    {
        if (!_viewModel.Game.ExecutePrimaryActionCommand.CanExecute(null))
        {
            return false;
        }

        _viewModel.Game.ExecutePrimaryActionCommand.Execute(null);
        return true;
    }

    private bool TryGetViewPoint(ItemsControl itemsControl, System.Windows.Point position, out SharpHack.Base.Model.Point targetView)
    {
        targetView = default;

        if (_viewModel.Game.ViewWidth <= 0 || _viewModel.Game.ViewHeight <= 0 || itemsControl.ActualWidth <= 0 || itemsControl.ActualHeight <= 0)
        {
            return false;
        }

        double cellWidth = itemsControl.ActualWidth / _viewModel.Game.ViewWidth;
        double cellHeight = itemsControl.ActualHeight / _viewModel.Game.ViewHeight;
        if (cellWidth <= 0 || cellHeight <= 0)
        {
            return false;
        }

        int viewX = Math.Clamp((int)(position.X / cellWidth), 0, _viewModel.Game.ViewWidth - 1);
        int viewY = Math.Clamp((int)(position.Y / cellHeight), 0, _viewModel.Game.ViewHeight - 1);
        targetView = new SharpHack.Base.Model.Point(viewX, viewY);
        return true;
    }
}
