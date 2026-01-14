using System;
using BaseLib.Interfaces;
using ConsoleDisplay.View;
using SharpHack.ViewModel;
using DrawingPoint = System.Drawing.Point;

namespace SharpHack.Console;

/// <summary>
/// Handles all console rendering.
/// </summary>
internal sealed class GameRenderer
{
    private readonly IConsole _console;
    private readonly GameViewModel _viewModel;
    private readonly TileDisplay<DisplayTile> _tileDisplay;
    private readonly Display _miniMap;
    private readonly Func<bool> _autoDoorOpenAccessor;
    private string? _lastPrimaryHint;

    public GameRenderer(
        IConsole console,
        GameViewModel viewModel,
        TileDisplay<DisplayTile> tileDisplay,
        Display miniMap,
        Func<bool> autoDoorOpenAccessor)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _tileDisplay = tileDisplay ?? throw new ArgumentNullException(nameof(tileDisplay));
        _miniMap = miniMap ?? throw new ArgumentNullException(nameof(miniMap));
        _autoDoorOpenAccessor = autoDoorOpenAccessor ?? throw new ArgumentNullException(nameof(autoDoorOpenAccessor));
    }

    /// <summary>
    /// Updates the tile display and HUD.
    /// </summary>
    public void Render()
    {
        _tileDisplay.Update(false);
        UpdatePrimaryActionHint();
        DrawHud();
        DrawMiniMap();
    }

    /// <summary>
    /// Provides tiles for the tile display.
    /// </summary>
    public DisplayTile GetTileAt(DrawingPoint point)
    {
        if (point.X < 0 || point.Y < 0 || point.X >= _viewModel.ViewWidth || point.Y >= _viewModel.ViewHeight)
        {
            return DisplayTile.Empty;
        }

        var index = point.Y * _viewModel.ViewWidth + point.X;
        return _viewModel.DisplayTiles[index];
    }

    private void DrawHud()
    {
        _console.ForegroundColor = ConsoleColor.White;
        _console.SetCursorPosition(0, ConsoleHudLayout.HudY);
        _console.Write($"HP: {_viewModel.HP}/{_viewModel.MaxHP}  Lvl: {_viewModel.Level}  AutoDoorOpen: {(_autoDoorOpenAccessor() ? "ON" : "OFF")}  ");
        _console.Write(new string(' ', 50));

        _console.SetCursorPosition(0, ConsoleHudLayout.HudY + 1);
        if (_viewModel.Messages.Count > 0)
        {
            var lastMessage = _viewModel.Messages[^1];
            _console.Write(lastMessage.PadRight(59));
        }
        else
        {
            _console.Write(new string(' ', 59));
        }
    }

    private void UpdatePrimaryActionHint()
    {
        var hint = _viewModel.PrimaryActionHint;
        if (hint == _lastPrimaryHint || hint == null)
        {
            return;
        }

        _lastPrimaryHint = hint;
        _viewModel.Messages.Add(hint);
    }

    private void DrawMiniMap()
    {
        var ratioX = ((_viewModel.Map.Width - 1) / _miniMap.dSize.Width) + 1;
        var ratioY = ((_viewModel.Map.Height - 1) / _miniMap.dSize.Height) + 1;
        var mm = _viewModel.MiniMap;

        for (var x = 0; x < _miniMap.dSize.Width; x++)
        {
            for (var y = 0; y < _miniMap.dSize.Height; y++)
            {
                var xMonster = false;
                var xWay = false;
                var xItem = false;
                var xExplored = false;
                var xPlayer = false;
                for (var xx = 0; xx < ratioX; xx++)
                {
                    for (var yy = 0; yy < ratioY; yy++)
                    {
                        var mapX = x * ratioX + xx;
                        var mapY = y * ratioY + yy;
                        if (mapX >= _viewModel.Map.Width || mapY >= _viewModel.Map.Height)
                        {
                            continue;
                        }

                        var tile = mm[mapY * _viewModel.Map.Width + mapX];
                        xPlayer |= (tile & 0x80) != 0;
                        xMonster |= (tile & 0x40) != 0;
                        xWay |= (tile & 0x4) != 0;
                        xItem |= (tile & 0x2) != 0;
                        xExplored |= (tile & 0x1) != 0;
                    }
                }

                var color = ConsoleColor.Black;
                if (xPlayer)
                    color = ConsoleColor.Yellow;
                else if (xMonster)
                    color = ConsoleColor.Red;
                else if (xWay)
                    color = ConsoleColor.Gray;
                else if (xItem)
                    color = ConsoleColor.Green;
                else if (xExplored)
                    color = ConsoleColor.DarkGray;

                _miniMap.PutPixel(x, y, color);
            }
        }

        _miniMap.Update();
    }
}
