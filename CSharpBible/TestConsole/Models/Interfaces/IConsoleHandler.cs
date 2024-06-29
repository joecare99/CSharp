using System;
using System.Drawing;
using TestConsole.View;

namespace TestConsole.Models.Interfaces;
public interface IConsoleHandler
{
    int WindowWidth { get; set; }
    int WindowHeight { get; set; }

    string Text { get; set; }
    ConsoleColor ForegroundColor { get; set; }
    ConsoleColor BackgroundColor { get; set; }
    bool KeyAvailable { get; }
    string Content { get; }
    ConsoleCharInfo[] ScreenBuffer { get; set; }
    Size ConsoleSize { get; set; }

    void Clear();
    (int Left, int Top) GetCursorPosition();
    ConsoleKeyInfo ReadKey();
    void SetCursorPosition(int left, int top);
    void Write(char ch);
    void Write(string? st);
    void YScroll(bool force = false);
}