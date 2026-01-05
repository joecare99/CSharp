using System;
using ConsoleDisplay.View;
using SharpHack.Engine;
using SharpHack.LevelGen;
using SharpHack.LevelGen.BSP;
using SharpHack.Base.Model;
using SharpHack.Combat;
using SharpHack.AI;
using SharpHack.ViewModel;
using BaseLib.Models; // Add using
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;

namespace SharpHack.Console;

public class Program
{
    private static GameViewModel _viewModel; // Use ViewModel instead of Session directly
    private static TileDisplay<DisplayTile> _tileDisplay;
    private static SharpHackTileDef _tileDef;
    private static Display _miniMap;
    private static int px;
    private static int py;

    public static void Main(string[] args)
    {
        Init();

        bool running = true;
        while (running)
        {
            Render();
            var key = System.Console.ReadKey(true).Key;
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

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
        _viewModel = new GameViewModel(session); // Initialize ViewModel

        _tileDef = new SharpHackTileDef();
        _tileDisplay = new TileDisplay<DisplayTile>(new ConsoleProxy(), _tileDef, DrawingPoint.Empty, new DrawingSize(70 / 4, 20 / 2), _tileDef.TileSize)
        {
            FncGetTile = GetTileAt,
            FncOldPos = GetOldPos,
        };
        TileDisplay<DisplayTile>.defaultTile = DisplayTile.Empty;

        var hudY = 20;
        _miniMap = new Display(60, 21, 20, 12);

        System.Console.CursorVisible = false;
        System.Console.Title = "SharpHack";
    }

    private static DrawingPoint GetOldPos(DrawingPoint point)
    {
       return  _viewModel.Map.GetOldPos(point.X,point.Y) is (int x,int y) p? new DrawingPoint(p.X,p.Y):DrawingPoint.Empty;
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
            case ConsoleKey.NumPad5: _viewModel.Wait(); break; // Wait command
            case ConsoleKey.Escape: return false;
        }
        return true;
    }

    private static void Render()
    {
        var p = _tileDisplay.DispOffset;
        if (_viewModel.Player.Position.X<10)
            p.X = 0;
        else if (_viewModel.Player.Position.X > _viewModel.ViewWidth -7)
            p.X = _viewModel.ViewWidth - 17;
        else
            p.X = _viewModel.Player.Position.X-10;
        if (_viewModel.Player.Position.Y < 5)
            p.Y = 0;
        else if (_viewModel.Player.Position.Y > _viewModel.ViewHeight- 5)
            p.Y = _viewModel.ViewHeight - 10;
        else
            p.Y = _viewModel.Player.Position.Y-5;
        _tileDisplay.DispOffset = p;

        _tileDisplay.Update(false);

        var hudY = 20;
        System.Console.ForegroundColor = System.ConsoleColor.White;
        System.Console.SetCursorPosition(0, hudY + 0);
        System.Console.Write($"HP: {_viewModel.HP}/{_viewModel.MaxHP}  Lvl: {_viewModel.Level}  ");
        System.Console.Write(new string(' ', 50));
        System.Console.SetCursorPosition(0, hudY + 1);

        if (_viewModel.Messages.Count > 0)
        {
            var lastMessage = _viewModel.Messages[_viewModel.Messages.Count - 1];
            System.Console.Write(lastMessage.PadRight(59));
        }
        else
        {
            System.Console.Write(new string(' ', 59));
        }

        DrawMiniMap();
    }

    private static void DrawMiniMap()
    {
        var ratioX = ((_viewModel.ViewWidth-1) / _miniMap.dSize.Width)+1;
        var ratioY = ((_viewModel.ViewHeight-1) / _miniMap.dSize.Height)+1;
        for (var x= 0;x < _miniMap.dSize.Width; x++)
        {
            for (var y= 0;y < _miniMap.dSize.Height; y++)
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
                        if (mapX >= _viewModel.ViewWidth || mapY >= _viewModel.ViewHeight)
                            continue;
                        if (mapX == _viewModel.Player.Position.X && mapY == _viewModel.Player.Position.Y)
                        {
                            xPlayer = true;
                            continue;
                        }
                        var tile = (int)_viewModel.DisplayTiles[mapY * _viewModel.ViewWidth + mapX];
                        if (tile is >= 0 and < (int)DisplayTile.Archaeologist)
                            xMonster = true;
                        else if (tile == (int)DisplayTile.Door_Closed 
                            ||  tile == (int)DisplayTile.Door_Open
                            ||  tile == (int)DisplayTile.Stairs_Down
                            ||  tile == (int)DisplayTile.Stairs_Up)
                            xWay = true;
                        else if (tile is >= (int)DisplayTile.Sword and <=  (int)DisplayTile.Armor)
                            xItem = true;
                        else if (tile != (int)DisplayTile.Empty)
                            xExplored = true;
                    }
                }
                if (xPlayer)
                    _miniMap.PutPixel(x, y,  ConsoleColor.Yellow);
                else if (xMonster)
                    _miniMap.PutPixel(x, y,  ConsoleColor.Red);
                else if (xWay)
                    _miniMap.PutPixel(x, y,  ConsoleColor.Gray);
                else if (xItem)
                    _miniMap.PutPixel(x, y,  ConsoleColor.Green);
                else if (xExplored)
                    _miniMap.PutPixel(x, y,  ConsoleColor.DarkGray);
                else
                    _miniMap.PutPixel(x, y,  ConsoleColor.Black);

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
