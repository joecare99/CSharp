using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using BaseLib.Interfaces;

namespace Ollama.CodingAgent.Console.Infrastructure;

/// <summary>
/// Provides the concrete <see cref="IConsole"/> host required by the ConsoleLib control ecosystem.
/// </summary>
/// <remarks>
/// ConsoleLib intentionally has no concrete widget backend in its core project. This adapter therefore
/// supplies its generic input/output contract to the thin line-oriented REPL instead of inventing a widget host.
/// </remarks>
public sealed class SystemConsoleAdapter : IConsole
{
    private readonly ISystemConsoleRuntime _runtime;
    private readonly IPlatformInfo _platform;

    /// <summary>
    /// Initializes a terminal adapter with process-console defaults or supplied deterministic seams.
    /// </summary>
    public SystemConsoleAdapter(
        ISystemConsoleRuntime? runtime = null,
        IPlatformInfo? platform = null)
    {
        _runtime = runtime ?? new SystemConsoleRuntime();
        _platform = platform ?? new SystemPlatformInfo();
    }

    /// <inheritdoc />
    public ConsoleColor ForegroundColor
    {
        get => _runtime.ForegroundColor;
        set => _runtime.ForegroundColor = value;
    }

    /// <inheritdoc />
    public ConsoleColor BackgroundColor
    {
        get => _runtime.BackgroundColor;
        set => _runtime.BackgroundColor = value;
    }

    /// <inheritdoc />
    public bool IsOutputRedirected => _runtime.IsOutputRedirected;

    /// <inheritdoc />
    public bool KeyAvailable => !_runtime.IsInputRedirected && _runtime.KeyAvailable;

    /// <inheritdoc />
    public int LargestWindowHeight => _runtime.LargestWindowHeight;

    /// <inheritdoc />
    public int LargestWindowWidth => _runtime.LargestWindowWidth;

    /// <inheritdoc />
    public string Title
    {
        get => _platform.IsWindows ? _runtime.Title : string.Empty;
        set
        {
            if (_platform.IsWindows)
            {
                _runtime.Title = value;
            }
        }
    }

    /// <inheritdoc />
    public int WindowHeight
    {
        get => _runtime.WindowHeight;
        set => _runtime.WindowHeight = value;
    }

    /// <inheritdoc />
    public int WindowWidth
    {
        get => _runtime.WindowWidth;
        set => _runtime.WindowWidth = value;
    }

    /// <inheritdoc />
    public int WindowLeft
    {
        get => _runtime.WindowLeft;
        set
        {
            if (_platform.IsWindows)
            {
                _runtime.WindowLeft = value;
            }
        }
    }

    /// <inheritdoc />
    public int WindowTop
    {
        get => _runtime.WindowTop;
        set
        {
            if (_platform.IsWindows)
            {
                _runtime.WindowTop = value;
            }
        }
    }

    /// <inheritdoc />
    public bool CursorVisible
    {
        get => _platform.IsWindows && _runtime.CursorVisible;
        set
        {
            if (_platform.IsWindows)
            {
                _runtime.CursorVisible = value;
            }
        }
    }

    /// <inheritdoc />
    public int BufferWidth => _runtime.BufferWidth;

    /// <inheritdoc />
    public int BufferHeight => _runtime.BufferHeight;

    /// <inheritdoc />
    public void Beep(int freq, int len)
    {
        if (_platform.IsWindows)
        {
            _runtime.Beep(freq, len);
        }
    }

    /// <inheritdoc />
    public void Clear() => _runtime.Clear();

    /// <inheritdoc />
    public (int Left, int Top) GetCursorPosition()
        => _runtime.GetCursorPosition();

    /// <inheritdoc />
    public ConsoleKeyInfo? ReadKey()
        => KeyAvailable ? _runtime.ReadKey() : null;

    /// <inheritdoc />
    public string ReadLine() => _runtime.ReadLine() ?? string.Empty;

    /// <inheritdoc />
    public void ResetColor() => _runtime.ResetColor();

    /// <inheritdoc />
    public void SetCursorPosition(int left, int top) => _runtime.SetCursorPosition(left, top);

    /// <inheritdoc />
    public void SetWindowPosition(int left, int top)
    {
        if (_platform.IsWindows)
        {
            _runtime.SetWindowPosition(left, top);
        }
    }

    /// <inheritdoc />
    public void SetWindowSize(int width, int height) => _runtime.SetWindowSize(width, height);

    /// <inheritdoc />
    public void Write(char ch) => _runtime.Write(ch);

    /// <inheritdoc />
    public void Write(string? st) => _runtime.Write(st);

    /// <inheritdoc />
    public void WriteLine(string? st = "") => _runtime.WriteLine(st);
}
