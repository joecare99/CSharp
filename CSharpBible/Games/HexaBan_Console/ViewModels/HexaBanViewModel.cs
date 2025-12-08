using HexaBan.Models.Interfaces;
using System;
namespace HexaBan_Console;

public class HexaBanViewModel
{
    private readonly IHexaBanEngine _engine;

    public HexaBanViewModel(IHexaBanEngine engine)
    {
        _engine = engine;
    }

    public TileType[,] Board => _engine.Board;
    public int Width => _engine.Width;
    public int Height => _engine.Height;
    public int Level => _engine.Level;
    public string Preview => _engine.Preview;
    public bool LevelBeendet => _engine.LevelBeendet;
    public void HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.A: _engine.Move(HexDirection.Left); break;
            case ConsoleKey.D: _engine.Move(HexDirection.Right); break;
            case ConsoleKey.W: _engine.Move(HexDirection.UpLeft); break;
            case ConsoleKey.E: _engine.Move(HexDirection.UpRight); break;
            case ConsoleKey.Y: _engine.Move(HexDirection.DownLeft); break;
            case ConsoleKey.X: _engine.Move(HexDirection.DownRight); break;
            case ConsoleKey.R: _engine.Reset(); break;
            case ConsoleKey.Enter when _engine.LevelBeendet: _engine.StartNextLevel(); break;
            default: break;
        }
    }
}
