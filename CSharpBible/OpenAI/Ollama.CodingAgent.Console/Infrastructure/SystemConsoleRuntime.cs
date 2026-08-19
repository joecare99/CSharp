using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent.Console.Infrastructure;

/// <summary>
/// Delegates terminal operations to the current process console.
/// </summary>
public sealed class SystemConsoleRuntime : ISystemConsoleRuntime
{
    private readonly IPlatformInfo _platform;
    private readonly Action<int> _setWindowLeft;
    private readonly Action<int> _setWindowTop;
    private readonly Action<bool> _setCursorVisible;
    private readonly Action<int, int> _setWindowPosition;
    private readonly Func<ConsoleKeyInfo> _readKey;
    private readonly Func<string?> _readLine;

    /// <summary>
    /// Initializes a process-console runtime with the current platform or a supplied platform seam.
    /// </summary>
    public SystemConsoleRuntime(
        IPlatformInfo? platform = null,
        Action<int>? setWindowLeft = null,
        Action<int>? setWindowTop = null,
        Action<bool>? setCursorVisible = null,
        Action<int, int>? setWindowPosition = null,
        Func<ConsoleKeyInfo>? readKey = null,
        Func<string?>? readLine = null)
    {
        _platform = platform ?? new SystemPlatformInfo();
        _setWindowLeft = setWindowLeft ?? CreatePropertySetter<int>(nameof(System.Console.WindowLeft));
        _setWindowTop = setWindowTop ?? CreatePropertySetter<int>(nameof(System.Console.WindowTop));
        _setCursorVisible = setCursorVisible ?? CreatePropertySetter<bool>(nameof(System.Console.CursorVisible));
        _setWindowPosition = setWindowPosition ?? CreateWindowPositionSetter();
        _readKey = readKey ?? (() => System.Console.ReadKey(intercept: true));
        _readLine = readLine ?? System.Console.ReadLine;
    }

    public ConsoleColor ForegroundColor
    {
        get => System.Console.ForegroundColor;
        set => System.Console.ForegroundColor = value;
    }

    public ConsoleColor BackgroundColor
    {
        get => System.Console.BackgroundColor;
        set => System.Console.BackgroundColor = value;
    }

    public bool IsOutputRedirected => System.Console.IsOutputRedirected;

    public bool IsInputRedirected => System.Console.IsInputRedirected;

    public bool KeyAvailable => System.Console.KeyAvailable;

    public int LargestWindowHeight => System.Console.LargestWindowHeight;

    public int LargestWindowWidth => System.Console.LargestWindowWidth;

    public string Title
    {
        get => _platform.IsWindows && OperatingSystem.IsWindows() ? System.Console.Title : string.Empty;
        set
        {
            if (_platform.IsWindows && OperatingSystem.IsWindows())
            {
                System.Console.Title = value;
            }
        }
    }

    public int WindowHeight
    {
        get => System.Console.WindowHeight;
        set => System.Console.WindowHeight = value;
    }

    public int WindowWidth
    {
        get => System.Console.WindowWidth;
        set => System.Console.WindowWidth = value;
    }

    public int WindowLeft
    {
        get => System.Console.WindowLeft;
        set
        {
            if (_platform.IsWindows && OperatingSystem.IsWindows())
            {
                _setWindowLeft(value);
            }
        }
    }

    public int WindowTop
    {
        get => System.Console.WindowTop;
        set
        {
            if (_platform.IsWindows && OperatingSystem.IsWindows())
            {
                _setWindowTop(value);
            }
        }
    }

    public bool CursorVisible
    {
        get => _platform.IsWindows && OperatingSystem.IsWindows() && System.Console.CursorVisible;
        set
        {
            if (_platform.IsWindows && OperatingSystem.IsWindows())
            {
                _setCursorVisible(value);
            }
        }
    }

    public int BufferWidth => System.Console.BufferWidth;

    public int BufferHeight => System.Console.BufferHeight;

    public void Beep(int frequency, int duration)
    {
        if (_platform.IsWindows && OperatingSystem.IsWindows())
        {
            System.Console.Beep(frequency, duration);
        }
    }

    public void Clear() => System.Console.Clear();

    public (int Left, int Top) GetCursorPosition()
        => (System.Console.CursorLeft, System.Console.CursorTop);

    public ConsoleKeyInfo ReadKey() => _readKey();

    public string? ReadLine() => _readLine();

    public void ResetColor() => System.Console.ResetColor();

    public void SetCursorPosition(int left, int top) => System.Console.SetCursorPosition(left, top);

    public void SetWindowPosition(int left, int top)
    {
        if (_platform.IsWindows && OperatingSystem.IsWindows())
        {
            _setWindowPosition(left, top);
        }
    }

    public void SetWindowSize(int width, int height) => System.Console.SetWindowSize(width, height);

    public void Write(char character) => System.Console.Write(character);

    public void Write(string? text) => System.Console.Write(text);

    public void WriteLine(string? text) => System.Console.WriteLine(text);

    private static Action<T> CreatePropertySetter<T>(string propertyName)
        => (Action<T>)typeof(System.Console)
            .GetProperty(propertyName)!
            .SetMethod!
            .CreateDelegate(typeof(Action<T>));

    private static Action<int, int> CreateWindowPositionSetter()
        => (Action<int, int>)typeof(System.Console)
            .GetMethod(nameof(System.Console.SetWindowPosition))!
            .CreateDelegate(typeof(Action<int, int>));
}
