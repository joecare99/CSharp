using System;
using System.Drawing;

namespace ConsoleLib.Interfaces;

/// <summary>Writes a deterministic terminal frame independent of a concrete host.</summary>
public interface IRenderContext
{
    Size Size { get; }
    void SetCell(int x, int y, TerminalCell cell);
    void Fill(Rectangle area, TerminalCell cell);
    void Invalidate(Rectangle area);
}
