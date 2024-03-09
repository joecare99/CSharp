using System.Drawing;
using System;
using System.ComponentModel;

namespace BlazorWasmDocker.Models.Interfaces;
public interface IConsoleHandler:INotifyPropertyChanged, INotifyPropertyChanging
{
    int WindowWidth { get; set; }
    int WindowHeight { get; set; }

    event EventHandler<EventArgs> DoUpdate;
    string Text { get; set; }
    ConsoleColor ForegroundColor { get; set; }
    ConsoleColor BackgroundColor { get; set; }

    Point CursorPosition { get; }

    bool KeyAvailable { get; }
    string Content { get; }
    ConsoleCharInfo[] ScreenBuffer { get; set; }
    Size ConsoleSize { get; set; }
    Color[] Ccolor { get; }

    void Clear();
    ConsoleKeyInfo ReadKey();
    void SetCursorPosition(int left, int top);
    void Write(char ch);
    void Write(string? st);
    void YScroll(bool force = false);
}