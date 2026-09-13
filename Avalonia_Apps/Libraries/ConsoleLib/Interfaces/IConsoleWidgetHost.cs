using ConsoleLib.CommonControls;
using System.Drawing;

namespace ConsoleLib.Interfaces;

public interface IConsoleWidgetHost : IHasConsoleSymbols, IHasConsoleKeyMap
{
    int WindowWidth { get; }
    bool IsOutputRedirected { get; }

    void ClearHost();
    void StopHost();
    void SetCursorPosition(int left, int top);
    void Beep(int frequency, int duration);
    void WriteTerminalCell(Terminal terminal, int x, int y, char character, System.ConsoleColor foreground, System.ConsoleColor background);
    void ClearTerminalCell(Terminal terminal, int x, int y, char character, System.ConsoleColor foreground, System.ConsoleColor background);
    void FillShadow(Rectangle rectangle, System.ConsoleColor foreground, System.ConsoleColor background, char fillChar);
}
