using BaseLib.Interfaces;
using System;
using System.Reflection;

namespace BaseLib.Models;

public class ConsoleProxy : IConsole
{
    protected MethodInfo _writeLineS = typeof(Console).GetMethod(nameof(Console.WriteLine), [typeof(string)])!;
    protected MethodInfo _beep = typeof(Console).GetMethod(nameof(Console.Beep), [typeof(int), typeof(int)])!;
#if NET5_0_OR_GREATER
    protected MethodInfo _GetCursorPosition = typeof(Console).GetMethod(nameof(Console.GetCursorPosition), [])!;
#else
    protected MethodInfo _CursorLeft = typeof(Console).GetMethod(nameof(Console.CursorLeft), [])!;
    protected MethodInfo _CursorTop = typeof(Console).GetMethod(nameof(Console.CursorTop), [])!;
#endif
    protected MethodInfo _ReadKey = typeof(Console).GetMethod(nameof(Console.ReadKey), [])!;
    protected MethodInfo _Readline = typeof(Console).GetMethod(nameof(Console.ReadLine), [])!;
    protected MethodInfo _setCursorPosition = typeof(Console).GetMethod(nameof(Console.SetCursorPosition), [typeof(int), typeof(int)])!;
    protected MethodInfo _writeCh = typeof(Console).GetMethod(nameof(Console.Write), [typeof(char)])!;
    protected MethodInfo _writeS = typeof(Console).GetMethod(nameof(Console.Write), [typeof(string)])!;
    protected MethodInfo _clear = typeof(Console).GetMethod(nameof(Console.Clear), [])!;
    protected MethodInfo _resetColor = typeof(Console).GetMethod(nameof(Console.ResetColor), [])!;

    protected PropertyInfo _ForegroundColor = typeof(Console).GetProperty(nameof(Console.ForegroundColor))!;
    protected PropertyInfo _BackgroundColor = typeof(Console).GetProperty(nameof(Console.BackgroundColor))!;
    protected PropertyInfo _IsOutputRedirected = typeof(Console).GetProperty(nameof(Console.IsOutputRedirected))!;
    protected PropertyInfo _KeyAvailable = typeof(Console).GetProperty(nameof(Console.KeyAvailable))!;
    protected PropertyInfo _LargestWindowHeight = typeof(Console).GetProperty(nameof(Console.LargestWindowHeight))!;
    protected PropertyInfo _Title = typeof(Console).GetProperty(nameof(Console.Title))!;
    protected PropertyInfo _WindowHeight = typeof(Console).GetProperty(nameof(Console.WindowHeight))!;
    protected PropertyInfo _WindowWidth = typeof(Console).GetProperty(nameof(Console.WindowWidth))!;
    protected PropertyInfo _CursorVisible = typeof(Console).GetProperty(nameof(Console.CursorVisible))!;
    protected PropertyInfo _BufferWidth = typeof(Console).GetProperty(nameof(Console.BufferWidth))!;
    protected PropertyInfo _BufferHeight = typeof(Console).GetProperty(nameof(Console.BufferHeight))!;

    protected object? instance = default;
    public ConsoleColor ForegroundColor { get => _ForegroundColor?.GetValue(instance) as ConsoleColor? ?? ConsoleColor.White; set => _ForegroundColor?.SetValue(instance,value); }
    public ConsoleColor BackgroundColor { get => _BackgroundColor?.GetValue(instance) as ConsoleColor? ?? ConsoleColor.Black; set => _BackgroundColor?.SetValue(instance, value); }
    public bool IsOutputRedirected => _IsOutputRedirected?.GetValue(instance) as bool? ?? false;
    public bool KeyAvailable => _KeyAvailable?.GetValue(instance) as bool? ?? false;
    public int LargestWindowHeight => _LargestWindowHeight?.GetValue(instance) as int? ?? 0;
    public string Title { get => _Title?.GetValue(instance) as string ?? string.Empty; set => _Title?.SetValue(instance, value); }
    public int WindowHeight { get => _WindowHeight?.GetValue(instance) as int? ?? 0; set => _WindowHeight?.SetValue(instance, value); }
    public int WindowWidth { get => _WindowWidth?.GetValue(instance) as int? ?? 0; set => _WindowWidth?.SetValue(instance, value); }
    public bool CursorVisible { get => _CursorVisible?.GetValue(instance) as bool? ?? false; set => _CursorVisible?.SetValue(instance, value); }
    public int BufferWidth => _BufferWidth?.GetValue(instance) as int? ?? 0;
    public int BufferHeight => _BufferHeight?.GetValue(instance) as int? ?? 0;


    public void Beep(int freq, int len) => _beep?.Invoke(instance, [freq, len]);
    public void Clear() => _clear?.Invoke(instance, []);
#if NET5_0_OR_GREATER
    public (int Left, int Top) GetCursorPosition() => _GetCursorPosition?.Invoke(instance, []) as (int, int)? ?? (0, 0);
#else
    public (int Left, int Top) GetCursorPosition() => (_CursorLeft?.Invoke(instance, []) as int? ?? 0, _CursorTop?.Invoke(instance, []) as int? ?? 0);
#endif
    public ConsoleKeyInfo? ReadKey() => _ReadKey?.Invoke(instance, []) as ConsoleKeyInfo?;
    public string ReadLine() => _Readline?.Invoke(instance, [])?.ToString() ?? string.Empty;

    public void ResetColor() => _resetColor?.Invoke(instance, []);

    public void SetCursorPosition(int left, int top) => _setCursorPosition?.Invoke(instance, [left, top]);
    public void Write(char ch) => _writeCh?.Invoke(instance, [ch]);
    public void Write(string? st) => _writeS?.Invoke(instance, [st]);
    public void WriteLine(string? v) => _writeLineS?.Invoke(instance, [v]);
}
