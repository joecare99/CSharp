using System;
using HexaBan.Models.Interfaces;
namespace HexaBan_Console;

public class HexaBanConsoleView
{
    private readonly HexaBanViewModel _viewModel;

    public HexaBanConsoleView(HexaBanViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Render(Action<ConsoleColor, ConsoleColor> SetColor,Action<string> Write)
    {
        int tileWidth = 4;
        int tileHeight = 2;

        Write($"Level: {_viewModel.Level} - {(_viewModel.LevelBeendet ? "Beendet" : "Laufend")}\r\n");

        for (int y = 0; y < _viewModel.Height; y++)
        {
            for (int line = 0; line < tileHeight; line++)
            {
                SetColor(ConsoleColor.Gray, ConsoleColor.Black);
                if (y % 2 == 1) Console.Write(new string(' ', tileWidth / 2));
                for (int x = 0; x < _viewModel.Width; x++)
                {
                    (string[] art, ConsoleColor fg, ConsoleColor bg) = TileToAsciiArt(_viewModel.Board[x, y]);
                    SetColor(fg, bg);
                    Write(art[line]);
                }
                Write("\r\n");
            }
        }
        if (_viewModel.LevelBeendet)
        {
            SetColor(ConsoleColor.Green, ConsoleColor.Black);
            Write("Drücke Enter für das nächste Level...\r\n");
            SetColor(ConsoleColor.Gray, ConsoleColor.Black);
            Write(_viewModel.Preview + "\r\n");
        }
        else
        {
            SetColor(ConsoleColor.Yellow, ConsoleColor.Black);
            Write("Steuerung: A=Links, D=Rechts, W=Oben-Links, E=Oben-Rechts, Y=Unten-Links, X=Unten-Rechts, R=Reset\r\n");
        }
        // Nach dem Rendern Farben zurücksetzen
    }

    private (string[] art, ConsoleColor fg, ConsoleColor bg) TileToAsciiArt(TileType tile)
    {
        // Unicode und Farben für mehr Kreativität
        return tile switch
        {
            TileType.Floor => (new[] { "░░░░", "░░░░" }, ConsoleColor.DarkGray, ConsoleColor.Black),
            TileType.Wall => (new[] { "▌▀▐▌", "▌▄▐▌" }, ConsoleColor.Red, ConsoleColor.DarkRed),
            TileType.Box => (new[] { "/_ /", "[_]/" }, ConsoleColor.Yellow, ConsoleColor.DarkYellow),
            TileType.Goal => (new[] { "    ", "+__+" }, ConsoleColor.Green, ConsoleColor.Black),
            TileType.Player => (new[] { "@\"°\\", "§L-<" }, ConsoleColor.Cyan, ConsoleColor.Black),
            TileType.BoxOnGoal => (new[] { "/_ /", "[X]/" }, ConsoleColor.Yellow, ConsoleColor.Green),
            TileType.PlayerOnGoal => (new[] { "°()°", "<__>" }, ConsoleColor.Cyan, ConsoleColor.Green),
            _ => (new[] { " ?? ", " ?? " }, ConsoleColor.Red, ConsoleColor.Black)
        };
    }
}
