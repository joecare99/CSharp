// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 07-16-2022
//
// Last Modified By : Mir
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="MyConsoleBase.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace BaseLib.Interfaces;

public interface IConsole
{
    ConsoleColor ForegroundColor { get; set; }
    ConsoleColor BackgroundColor { get; set; }
    bool IsOutputRedirected { get; }
    bool KeyAvailable { get; }
    int LargestWindowHeight { get; }
    string Title { get; set; }
    int WindowHeight { get; set; }
    int WindowWidth { get; set; }

    void Beep(int freq, int len);
    void Clear();
    (int Left, int Top) GetCursorPosition();
    ConsoleKeyInfo? ReadKey();
    string ReadLine();
    void SetCursorPosition(int left, int top);
    void Write(char ch);
    void Write(string? st);
    void WriteLine(string? st = "");
}