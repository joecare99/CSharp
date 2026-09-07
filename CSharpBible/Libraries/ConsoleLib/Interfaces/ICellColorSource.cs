using System;

namespace ConsoleLib.Interfaces;

/// <summary>Provides per-cell colors for a text control.</summary>
public interface ICellColorSource
{
    ConsoleColor GetCellForeground(int x, int y);
    ConsoleColor GetCellBackground(int x, int y);
}
