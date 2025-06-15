using System.Collections.Generic;
using HexaBan.Models;
using HexaBan.Models.Interfaces;

namespace HexaBan.Data;

public static class HexaBanLevels
{
    // Jede Level-Vorschau wird als Text-Grafik im Preview-String erzeugt, z.B.:
    //   . @ o
    //    . x .
    //     . . .
    // Die Vorschau zeigt das Spielfeld als ASCII-Art, wobei:
    //   @ = Spieler, o = Kiste, x = Ziel, * = Kiste auf Ziel, # = Wand, . = Boden
    public static IEnumerable<(Dictionary<HexCoord, HexTile> Board, HexCoord PlayerStart, string Preview)> GetLevels()
    {
        // Hilfsfunktion zum Erstellen eines leeren Boards
        Dictionary<HexCoord, HexTile> EmptyBoard(int width, int height)
        {
            var board = new Dictionary<HexCoord, HexTile>();
            for (int q = 0; q < width; q++)
                for (int r = 0; r < height; r++)
                    board[new HexCoord(q, r)] = new HexTile { Type = TileType.Floor };
            return board;
        }

        // Hilfsfunktion für die Text-Grafik
        string RenderBoard(Dictionary<HexCoord, HexTile> board, int width, int height, HexCoord playerStart)
        {
            var lines = new List<string>();
            for (int r = 0; r < height; r++)
            {
                var line = new System.Text.StringBuilder();
                line.Append(new string(' ', r)); // für Hex-Optik
                for (int q = 0; q < width; q++)
                {
                    var coord = new HexCoord(q, r);
                    if (!board.TryGetValue(coord, out var tile))
                    {
                        line.Append(' ');
                        continue;
                    }
                    char c = '.';
                    if (playerStart.Q == q && playerStart.R == r)
                        c = tile.HasBox ? '*' : '@';
                    else if (tile.HasBox && tile.Type == TileType.Goal)
                        c = '*';
                    else if (tile.HasBox)
                        c = 'o';
                    else if (tile.Type == TileType.Goal)
                        c = 'x';
                    else if (tile.Type == TileType.Wall)
                        c = '#';
                    line.Append(c);
                    line.Append(' ');
                }
                lines.Add(line.ToString().TrimEnd());
            }
            return string.Join("\r\n", lines);
        }


        // Level 1: Einfachster Fall, 1 Kiste, 1 Ziel
        {
            var board = EmptyBoard(3, 3);
            board[new HexCoord(1, 1)].Type = TileType.Goal;
            board[new HexCoord(1, 0)].HasBox = true;
            var playerStart = new HexCoord(0, 1);
            var preview = RenderBoard(board, 3, 3, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 2: Kiste muss um eine Ecke geschoben werden
        {
            var board = EmptyBoard(4, 3);
            board[new HexCoord(2, 2)].Type = TileType.Goal;
            board[new HexCoord(1, 1)].HasBox = true;
            var playerStart = new HexCoord(0, 1);
            var preview = RenderBoard(board, 4, 3, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 3: Wand blockiert direkten Weg
        {
            var board = EmptyBoard(4, 3);
            board[new HexCoord(2, 2)].Type = TileType.Goal;
            board[new HexCoord(1, 1)].HasBox = true;
            board[new HexCoord(1, 2)].Type = TileType.Wall;
            var playerStart = new HexCoord(0, 1);
            var preview = RenderBoard(board, 4, 3, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 4: Zwei Kisten, zwei Ziele
        {
            var board = EmptyBoard(4, 4);
            board[new HexCoord(2, 2)].Type = TileType.Goal;
            board[new HexCoord(1, 2)].Type = TileType.Goal;
            board[new HexCoord(1, 1)].HasBox = true;
            board[new HexCoord(2, 1)].HasBox = true;
            var playerStart = new HexCoord(0, 2);
            var preview = RenderBoard(board, 4, 4, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 5: Kiste muss um Hindernis herum
        {
            var board = EmptyBoard(5, 4);
            board[new HexCoord(3, 2)].Type = TileType.Goal;
            board[new HexCoord(1, 1)].HasBox = true;
            board[new HexCoord(2, 1)].Type = TileType.Wall;
            var playerStart = new HexCoord(0, 1);
            var preview = RenderBoard(board, 5, 4, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 6: Zwei Kisten, Ziele diagonal
        {
            var board = EmptyBoard(5, 5);
            board[new HexCoord(3, 3)].Type = TileType.Goal;
            board[new HexCoord(1, 1)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(2, 1)].HasBox = true;
            var playerStart = new HexCoord(0, 2);
            var preview = RenderBoard(board, 5, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 7: Enger Gang
        {
            var board = EmptyBoard(5, 3);
            board[new HexCoord(4, 1)].Type = TileType.Goal;
            board[new HexCoord(2, 1)].HasBox = true;
            board[new HexCoord(3, 1)].Type = TileType.Wall;
            var playerStart = new HexCoord(0, 1);
            var preview = RenderBoard(board, 5, 3, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 8: Zwei Kisten, eine blockiert die andere
        {
            var board = EmptyBoard(5, 4);
            board[new HexCoord(3, 2)].Type = TileType.Goal;
            board[new HexCoord(4, 2)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 1)].HasBox = true;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 5, 4, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 9: Drei Kisten, drei Ziele
        {
            var board = EmptyBoard(5, 5);
            board[new HexCoord(1, 3)].Type = TileType.Goal;
            board[new HexCoord(3, 3)].Type = TileType.Goal;
            board[new HexCoord(2, 4)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(2, 3)].HasBox = true;
            board[new HexCoord(3, 2)].HasBox = true;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 5, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 10: Sackgasse vermeiden
        {
            var board = EmptyBoard(5, 5);
            board[new HexCoord(4, 2)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 2)].Type = TileType.Wall;
            board[new HexCoord(3, 1)].Type = TileType.Wall;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 5, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 11: Kiste muss um mehrere Wände
        {
            var board = EmptyBoard(6, 5);
            board[new HexCoord(5, 2)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 2)].Type = TileType.Wall;
            board[new HexCoord(4, 2)].Type = TileType.Wall;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 6, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 12: Zwei Kisten, Ziele nebeneinander
        {
            var board = EmptyBoard(6, 4);
            board[new HexCoord(4, 2)].Type = TileType.Goal;
            board[new HexCoord(5, 2)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 2)].HasBox = true;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 6, 4, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 13: Drei Kisten, Ziele in Linie
        {
            var board = EmptyBoard(6, 5);
            board[new HexCoord(4, 2)].Type = TileType.Goal;
            board[new HexCoord(5, 2)].Type = TileType.Goal;
            board[new HexCoord(3, 2)].Type = TileType.Goal;
            board[new HexCoord(1, 2)].HasBox = true;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 1)].HasBox = true;
            var playerStart = new HexCoord(0, 2);
            var preview = RenderBoard(board, 6, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 14: Kiste muss um Ecke und Wand
        {
            var board = EmptyBoard(6, 5);
            board[new HexCoord(5, 3)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 2)].Type = TileType.Wall;
            board[new HexCoord(4, 3)].Type = TileType.Wall;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 6, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 15: Vier Kisten, vier Ziele
        {
            var board = EmptyBoard(7, 6);
            board[new HexCoord(5, 4)].Type = TileType.Goal;
            board[new HexCoord(6, 4)].Type = TileType.Goal;
            board[new HexCoord(5, 5)].Type = TileType.Goal;
            board[new HexCoord(6, 5)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 2)].HasBox = true;
            board[new HexCoord(2, 3)].HasBox = true;
            board[new HexCoord(3, 3)].HasBox = true;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 7, 6, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 16: Enges Labyrinth
        {
            var board = EmptyBoard(7, 5);
            board[new HexCoord(6, 2)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(3, 2)].Type = TileType.Wall;
            board[new HexCoord(4, 2)].Type = TileType.Wall;
            board[new HexCoord(5, 2)].Type = TileType.Wall;
            var playerStart = new HexCoord(1, 2);
            var preview = RenderBoard(board, 7, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 17: Zwei Kisten, Ziele weit auseinander
        {
            var board = EmptyBoard(7, 5);
            board[new HexCoord(6, 4)].Type = TileType.Goal;
            board[new HexCoord(0, 0)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(4, 3)].HasBox = true;
            var playerStart = new HexCoord(3, 1);
            var preview = RenderBoard(board, 7, 5, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 18: Drei Kisten, Sackgassen
        {
            var board = EmptyBoard(7, 6);
            board[new HexCoord(6, 5)].Type = TileType.Goal;
            board[new HexCoord(0, 0)].Type = TileType.Goal;
            board[new HexCoord(3, 3)].Type = TileType.Goal;
            board[new HexCoord(2, 2)].HasBox = true;
            board[new HexCoord(4, 4)].HasBox = true;
            board[new HexCoord(1, 1)].HasBox = true;
            board[new HexCoord(5, 5)].Type = TileType.Wall;
            var playerStart = new HexCoord(3, 1);
            var preview = RenderBoard(board, 7, 6, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 19: Vier Kisten, Ziele in Ecken
        {
            var board = EmptyBoard(8, 8);
            board[new HexCoord(0, 0)].Type = TileType.Goal;
            board[new HexCoord(7, 0)].Type = TileType.Goal;
            board[new HexCoord(0, 7)].Type = TileType.Goal;
            board[new HexCoord(7, 7)].Type = TileType.Goal;
            board[new HexCoord(3, 3)].HasBox = true;
            board[new HexCoord(4, 4)].HasBox = true;
            board[new HexCoord(3, 4)].HasBox = true;
            board[new HexCoord(4, 3)].HasBox = true;
            var playerStart = new HexCoord(2, 2);
            var preview = RenderBoard(board, 8, 8, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 20: Viele Wände, enge Wege
        {
            var board = EmptyBoard(8, 8);
            board[new HexCoord(7, 7)].Type = TileType.Goal;
            board[new HexCoord(6, 6)].Type = TileType.Goal;
            board[new HexCoord(5, 5)].Type = TileType.Goal;
            board[new HexCoord(4, 4)].HasBox = true;
            board[new HexCoord(3, 3)].HasBox = true;
            board[new HexCoord(2, 2)].HasBox = true;
            for (int i = 1; i < 7; i++)
                board[new HexCoord(i, i)].Type = TileType.Wall;
            var playerStart = new HexCoord(0, 0);
            var preview = RenderBoard(board, 8, 8, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 21: Kisten müssen in bestimmter Reihenfolge
        {
            var board = EmptyBoard(8, 8);
            board[new HexCoord(7, 7)].Type = TileType.Goal;
            board[new HexCoord(6, 6)].Type = TileType.Goal;
            board[new HexCoord(5, 5)].Type = TileType.Goal;
            board[new HexCoord(4, 4)].HasBox = true;
            board[new HexCoord(5, 4)].HasBox = true;
            board[new HexCoord(6, 4)].HasBox = true;
            board[new HexCoord(3, 3)].Type = TileType.Wall;
            board[new HexCoord(4, 3)].Type = TileType.Wall;
            var playerStart = new HexCoord(2, 2);
            var preview = RenderBoard(board, 8, 8, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 22: Viele Kisten, viele Ziele
        {
            var board = EmptyBoard(9, 9);
            for (int i = 0; i < 5; i++)
            {
                board[new HexCoord(8, i)].Type = TileType.Goal;
                board[new HexCoord(0, 8 - i)].Type = TileType.Goal;
                board[new HexCoord(4, i + 2)].HasBox = true;
            }
            var playerStart = new HexCoord(4, 0);
            var preview = RenderBoard(board, 9, 9, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 23: Kisten in Sackgassen
        {
            var board = EmptyBoard(9, 9);
            board[new HexCoord(8, 8)].Type = TileType.Goal;
            board[new HexCoord(0, 0)].Type = TileType.Goal;
            board[new HexCoord(4, 4)].Type = TileType.Goal;
            board[new HexCoord(1, 1)].HasBox = true;
            board[new HexCoord(7, 7)].HasBox = true;
            board[new HexCoord(4, 5)].HasBox = true;
            board[new HexCoord(2, 2)].Type = TileType.Wall;
            board[new HexCoord(6, 6)].Type = TileType.Wall;
            var playerStart = new HexCoord(4, 0);
            var preview = RenderBoard(board, 9, 9, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 24: Viele Wände, viele Kisten
        {
            var board = EmptyBoard(10, 10);
            for (int i = 0; i < 5; i++)
            {
                board[new HexCoord(9, i)].Type = TileType.Goal;
                board[new HexCoord(0, 9 - i)].Type = TileType.Goal;
                board[new HexCoord(5, i + 2)].HasBox = true;
                board[new HexCoord(i, 5)].Type = TileType.Wall;
            }
            var playerStart = new HexCoord(5, 0);
            var preview = RenderBoard(board, 10, 10, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 25: Ziele in der Mitte, Kisten außen
        {
            var board = EmptyBoard(10, 10);
            for (int i = 3; i < 7; i++)
            {
                board[new HexCoord(5, i)].Type = TileType.Goal;
                board[new HexCoord(i, 5)].HasBox = true;
            }
            var playerStart = new HexCoord(0, 0);
            var preview = RenderBoard(board, 10, 10, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 26: Viele Kisten, viele Ziele, viele Wände
        {
            var board = EmptyBoard(10, 10);
            for (int i = 0; i < 5; i++)
            {
                board[new HexCoord(9, i)].Type = TileType.Goal;
                board[new HexCoord(0, 9 - i)].Type = TileType.Goal;
                board[new HexCoord(5, i + 2)].HasBox = true;
                board[new HexCoord(i, 5)].Type = TileType.Wall;
                board[new HexCoord(9 - i, 4)].Type = TileType.Wall;
            }
            var playerStart = new HexCoord(5, 0);
            var preview = RenderBoard(board, 10, 10, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 27: Sehr viele Kisten, Ziele und Wände
        {
            var board = EmptyBoard(12, 12);
            for (int i = 0; i < 6; i++)
            {
                board[new HexCoord(11, i)].Type = TileType.Goal;
                board[new HexCoord(0, 11 - i)].Type = TileType.Goal;
                board[new HexCoord(6, i + 3)].HasBox = true;
                board[new HexCoord(i, 6)].Type = TileType.Wall;
                board[new HexCoord(11 - i, 5)].Type = TileType.Wall;
            }
            var playerStart = new HexCoord(6, 0);
            var preview = RenderBoard(board, 12, 12, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 28: Ziele und Kisten im Zickzack
        {
            var board = EmptyBoard(12, 12);
            for (int i = 0; i < 6; i++)
            {
                board[new HexCoord(i * 2, i)].Type = TileType.Goal;
                board[new HexCoord(i * 2 + 1, i)].HasBox = true;
            }
            var playerStart = new HexCoord(0, 0);
            var preview = RenderBoard(board, 12, 12, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 29: Sehr viele Kisten, Ziele, Wände, komplexes Muster
        {
            var board = EmptyBoard(14, 14);
            for (int i = 0; i < 7; i++)
            {
                board[new HexCoord(13, i)].Type = TileType.Goal;
                board[new HexCoord(0, 13 - i)].Type = TileType.Goal;
                board[new HexCoord(7, i + 4)].HasBox = true;
                board[new HexCoord(i, 7)].Type = TileType.Wall;
                board[new HexCoord(13 - i, 6)].Type = TileType.Wall;
            }
            var playerStart = new HexCoord(7, 0);
            var preview = RenderBoard(board, 14, 14, playerStart);
            yield return (board, playerStart, preview);
        }

        // Level 30: Maximale Komplexität
        {
            var board = EmptyBoard(16, 16);
            for (int i = 0; i < 8; i++)
            {
                board[new HexCoord(15, i)].Type = TileType.Goal;
                board[new HexCoord(0, 15 - i)].Type = TileType.Goal;
                board[new HexCoord(8, i + 5)].HasBox = true;
                board[new HexCoord(i, 8)].Type = TileType.Wall;
                board[new HexCoord(15 - i, 7)].Type = TileType.Wall;
            }
            var playerStart = new HexCoord(8, 0);
            var preview = RenderBoard(board, 16, 16, playerStart);
            yield return (board, playerStart, preview);
        }
    }
}
