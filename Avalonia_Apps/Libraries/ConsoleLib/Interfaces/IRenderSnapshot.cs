using System.Drawing;

namespace ConsoleLib.Interfaces;

/// <summary>Read-only snapshot of a rendered terminal frame.</summary>
public interface IRenderSnapshot
{
    Size Size { get; }
    TerminalCell GetCell(int x, int y);
    bool IsInvalidated(Rectangle area);
}
