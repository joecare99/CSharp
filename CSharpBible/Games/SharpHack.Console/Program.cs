using System;
using ConsoleDisplay.View;
using SharpHack.Engine;
using SharpHack.Base.Model;
using SharpHack.Combat;
using SharpHack.AI;
using SharpHack.ViewModel;
using BaseLib.Models; // Add using
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;
using SharpHack.Persist;
using SharpHack.LevelGen.BSP;
using SharpHack.Base.Data;
using BaseLib.Interfaces;

namespace SharpHack.Console;

public class Program
{
    private static readonly IConsole _console = new ConsoleProxy();

    private static GameViewModel _viewModel; // Use ViewModel instead of Session directly
    private static TileDisplay<DisplayTile> _tileDisplay;
    private static ITileDef _tileDef;
    private static Display _miniMap;
    private static int px;
    private static int py;

    private static bool _autoDoorOpen = true;

    private static string? _lastPrimaryHint;

    private const int HudY = 20;

    public static void Main(string[] args)
    {
        Init();

        bool running = true;
        while (running)
        {
            Render();
            var key = _console.ReadKey()?.Key ?? ConsoleKey.NoName;
            running = HandleInput(key);
        }
    }

    private static void Init()
    {
        // Setup dependencies
        var random = new CRandom();
        var mapGenerator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI();
        var gamePersist = new InMemoryGamePersist();

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        _viewModel = new GameViewModel(session); // Initialize ViewModel
        _tileDef = new TileDefRes(".\\Resources\\Tiles4x2.tdj");
        _viewModel.SetViewSize(70 / _tileDef.TileSize.Width, 20 / _tileDef.TileSize.Height);
        _tileDisplay = new TileDisplay<DisplayTile>(_console, _tileDef, DrawingPoint.Empty,
            new DrawingSize(70 / _tileDef.TileSize.Width, 20 / _tileDef.TileSize.Height), _tileDef.TileSize)
        {
            FncGetTile = GetTileAt,
            //    FncOldPos = GetOldPos,
        };
        TileDisplay<DisplayTile>.defaultTile = DisplayTile.Empty;

        _miniMap = new Display(60, 21, 20, 12);

        _console.CursorVisible = false;
        _console.Title = "SharpHack";
    }

    private static bool HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow: _viewModel.Move(Direction.North); break;
            case ConsoleKey.DownArrow: _viewModel.Move(Direction.South); break;
            case ConsoleKey.LeftArrow: _viewModel.Move(Direction.West); break;
            case ConsoleKey.RightArrow: _viewModel.Move(Direction.East); break;
            case ConsoleKey.NumPad7: _viewModel.Move(Direction.NorthWest); break;
            case ConsoleKey.NumPad9: _viewModel.Move(Direction.NorthEast); break;
            case ConsoleKey.NumPad1: _viewModel.Move(Direction.SouthWest); break;
            case ConsoleKey.NumPad3: _viewModel.Move(Direction.SouthEast); break;
            case ConsoleKey.NumPad5:
            case ConsoleKey.OemPeriod:
                _viewModel.Wait();
                break;

            // Targeted commands
            case ConsoleKey.O:
                _viewModel.OpenDoorNearby();
                break;
            case ConsoleKey.C:
                _viewModel.CloseDoorNearby();
                break;
            case ConsoleKey.T:
                _viewModel.ToggleDoorNearby();
                break;

            case ConsoleKey.A:
                _autoDoorOpen = !_autoDoorOpen;
                _viewModel.AutoDoorOpen = _autoDoorOpen;
                _viewModel.Messages.Add($"AutoDoorOpen: {(_autoDoorOpen ? "ON" : "OFF")}");
                break;

            case ConsoleKey.Enter:
                _viewModel.ExecutePrimaryAction();
                break;

            case ConsoleKey.Escape:
                return false;

            case ConsoleKey.G:
                GoRelative();
                break;
        }

        return true;
    }

    private static void GoRelative()
    {
        var pp = _viewModel.Player.Position;

        var (dx, dy) = ReadIntPairAtHud($"GoTo (dx,dy) from [{pp.X},{pp.Y}]: ");

        var target = new Point(pp.X + dx, pp.Y + dy);

        if (!_viewModel.Map.IsValid(target))
        {
            _viewModel.Messages.Add("Target is outside of the map.");
            return;
        }

        _viewModel.GoToWorldAsync(target).GetAwaiter().GetResult();
    }

    private static (int dx, int dy) ReadIntPairAtHud(string prompt)
    {
        var prevCursorVisible = _console.CursorVisible;
        var prevFore = _console.ForegroundColor;
        var prevBack = _console.BackgroundColor;

        try
        {
            _console.CursorVisible = true;
            _console.ForegroundColor = ConsoleColor.White;
            _console.BackgroundColor = ConsoleColor.Black;

            while (true)
            {
                // Render once so we don't input over stale map frames
                Render();

                // Prompt on HUD message line
                _console.SetCursorPosition(0, HudY + 1);
                _console.Write(new string(' ', 79));
                _console.SetCursorPosition(0, HudY + 1);
                _console.Write(prompt);

                // place cursor after prompt
                var inputX = Math.Min(_console.BufferWidth - 1, prompt.Length);
                _console.SetCursorPosition(inputX, HudY + 1);

                var s = _console.ReadLine();
                if (string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                var parts = s.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    continue;
                }

                if (int.TryParse(parts[0], out var dx) && int.TryParse(parts[1], out var dy))
                {
                    return (dx, dy);
                }
            }
        }
        finally
        {
            _console.ForegroundColor = prevFore;
            _console.BackgroundColor = prevBack;
            _console.CursorVisible = prevCursorVisible;
        }
    }

    private static void Render()
    {
        _tileDisplay.Update(false);

        UpdatePrimaryActionHint();

        _console.ForegroundColor = ConsoleColor.White;
        _console.SetCursorPosition(0, HudY + 0);
        _console.Write($"HP: {_viewModel.HP}/{_viewModel.MaxHP}  Lvl: {_viewModel.Level}  AutoDoorOpen: {(_autoDoorOpen ? "ON" : "OFF")}  ");
        _console.Write(new string(' ', 50));
        _console.SetCursorPosition(0, HudY + 1);

        if (_viewModel.Messages.Count > 0)
        {
            var lastMessage = _viewModel.Messages[_viewModel.Messages.Count - 1];
            _console.Write(lastMessage.PadRight(59));
        }
        else
        {
            _console.Write(new string(' ', 59));
        }

        DrawMiniMap();
    }

    private static void UpdatePrimaryActionHint()
    {
        var hint = _viewModel.PrimaryActionHint;
        if (hint != _lastPrimaryHint)
        {
            _lastPrimaryHint = hint;
            if (hint != null)
            {
                _viewModel.Messages.Add(hint);
            }
        }
    }

    private static IEnumerable<Point> GetAdjacent8(Point p)
    {
        yield return new Point(p.X - 1, p.Y - 1);
        yield return new Point(p.X, p.Y - 1);
        yield return new Point(p.X + 1, p.Y - 1);
        yield return new Point(p.X - 1, p.Y);
        yield return new Point(p.X + 1, p.Y);
        yield return new Point(p.X - 1, p.Y + 1);
        yield return new Point(p.X, p.Y + 1);
        yield return new Point(p.X + 1, p.Y + 1);
    }

    private static void DrawMiniMap()
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
                            continue;
                        var tile = mm[mapY * _viewModel.Map.Width + mapX];
                        xPlayer |= (tile & 0x80) != 0;
                        xMonster |= (tile & 0x40) != 0;
                        // xWall = (tile & 0x8) != 0; 
                        xWay |= (tile & 0x4) != 0;
                        xItem |= (tile & 0x2) != 0;
                        xExplored |= (tile & 0x1) != 0;
                    }
                }
                if (xPlayer)
                    _miniMap.PutPixel(x, y, ConsoleColor.Yellow);
                else if (xMonster)
                    _miniMap.PutPixel(x, y, ConsoleColor.Red);
                else if (xWay)
                    _miniMap.PutPixel(x, y, ConsoleColor.Gray);
                else if (xItem)
                    _miniMap.PutPixel(x, y, ConsoleColor.Green);
                else if (xExplored)
                    _miniMap.PutPixel(x, y, ConsoleColor.DarkGray);
                else
                    _miniMap.PutPixel(x, y, ConsoleColor.Black);

            }
        }
        _miniMap.Update();
    }

    private static DisplayTile GetTileAt(DrawingPoint p)
    {
        if ((p.X) < 0 || (p.Y) < 0 || (p.X) >= _viewModel.ViewWidth || (p.Y) >= _viewModel.ViewHeight)
            return DisplayTile.Empty;

        var index = (p.Y) * _viewModel.ViewWidth + p.X;
        return _viewModel.DisplayTiles[index];
    }
}
