using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent.Console.Interfaces;

/// <summary>
/// Abstracts process-console operations so terminal presentation code remains deterministic to test.
/// </summary>
public interface ISystemConsoleRuntime
{
    ConsoleColor ForegroundColor { get; set; }

    ConsoleColor BackgroundColor { get; set; }

    bool IsOutputRedirected { get; }

    bool IsInputRedirected { get; }

    bool KeyAvailable { get; }

    int LargestWindowHeight { get; }

    int LargestWindowWidth { get; }

    string Title { get; set; }

    int WindowHeight { get; set; }

    int WindowWidth { get; set; }

    int WindowLeft { get; set; }

    int WindowTop { get; set; }

    bool CursorVisible { get; set; }

    int BufferWidth { get; }

    int BufferHeight { get; }

    void Beep(int frequency, int duration);

    void Clear();

    (int Left, int Top) GetCursorPosition();

    ConsoleKeyInfo ReadKey();

    string? ReadLine();

    void ResetColor();

    void SetCursorPosition(int left, int top);

    void SetWindowPosition(int left, int top);

    void SetWindowSize(int width, int height);

    void Write(char character);

    void Write(string? text);

    void WriteLine(string? text);
}
